using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Chandler.Controllers
{
    /// <summary>
    /// Api Controller For Threads
    /// </summary>
    [ApiController, Route("api/[controller]"), Produces("application/json"), AllowAnonymous]
    public class ThreadController : Controller
    {
        private readonly Database Database;
        private readonly ServerConfig Config;

        /// <summary>
        /// Thread API Ctor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="conf"></param>
        public ThreadController(Database database, ServerConfig conf)
        {
            this.Database = database;
            this.Config = conf;
        }

        /// <summary>
        /// Returns a list of threads for a given board
        /// </summary>
        /// <param name="tag">The board tag</param>
        /// <param name="includechildren">Whether or not to include the children to a thread</param>
        /// <returns>IEnunmerable of Thread or empty</returns>
        /// <response code="200">If the board was found successfully</response>
        [HttpGet, ProducesResponseType(200)]
        public ActionResult<IEnumerable<Thread>> GetThreads([FromQuery]string tag, [FromQuery]bool includechildren = false)
        {
            var boardthreads = this.Database.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1);
            if (boardthreads.Count() > 0)
            {
                if (!includechildren) return boardthreads.OrderBy(x => x.Id).ToList();
                else
                {
                    var childedthreads = boardthreads.OrderBy(x => x.Id).ToList();
                    childedthreads.ForEach(x => x.ChildThreads = boardthreads.Where(a => a.ParentId == x.Id));
                    return childedthreads;
                }
            }
            else return this.NotFound("Board not found or the board has no threads");
        }

        /// <summary>
        /// Gets a thread by its ID
        /// </summary>
        /// <param name="id">The ID of the thread</param>
        /// <param name="includechildren">Whether or not to include the children to a thread</param>
        /// <returns>Thread or empty</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("single"), ProducesResponseType(200)]
        public ActionResult<Thread> GetSingleThread([FromQuery]int id, [FromQuery]bool includechildren = false)
        {
            var thread = this.Database.Threads.FirstOrDefault(x => x.Id == id);
            if (thread != null)
            {
                if (!includechildren) return thread;
                else
                {
                    thread.ChildThreads = this.Database.Threads.Where(x => x.ParentId == thread.Id);
                    return thread;
                }
            }
            else return NotFound("No thread with the given ID was found");
        }

        /// <summary>
        /// Gets child posts on a given thread
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of children threads (IEnumerable of Thread)</returns>
        /// <response code="200">If the thread was found</response>
        [HttpGet("children"), ProducesResponseType(200)]
        public ActionResult<IEnumerable<Thread>> GetPosts([FromQuery]int id)
        {
            if (id < 1) return BadRequest("ID must be valid");
            else return this.Database.Threads.Where(x => x.ParentId == id).OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Creates a post from post body content
        /// </summary>
        /// <param name="newpost">Thread Body</param>
        /// <returns>Newly posted thread</returns>
        /// <response code="200">If the thread was created successfully</response>
        [HttpPost("create"), ProducesResponseType(200)]
        public async Task<ActionResult<Thread>> CreatePost([FromBody] Thread newpost)
        {
            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("Text body is too short");

            if (newpost.IsCommentReply && newpost.ParentId == -1)
                return BadRequest("Expected a comment, got a parent");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            var passw = newpost.GeneratePassword;
            int passid = -1;
            if (!string.IsNullOrWhiteSpace(passw))
            {
                (var hash, var salt) = Passworder.GenerateHash(passw, this.Config.DefaultPassword);
                var newpass = new Password()
                {
                    Hash = hash,
                    Salt = salt
                };
                this.Database.Passwords.Add(newpass);
                this.Database.SaveChanges();
                passid = newpass.Id;
            }

            newpost.GeneratePassword = "";
            newpost.PasswordId = passid;

            await this.Database.Threads.AddAsync(newpost);
            await this.Database.SaveChangesAsync();

            //Run in background
            await Task.Run(async () => await Webhooker.SendContentToAllAsync(this.Database, newpost));

            return newpost;
        }

        /// <summary>
        /// Delete a post via delete request type
        /// </summary>
        /// <param name="postid">ID of the most to delete</param>
        /// <param name="pass">Password to the thread</param>
        /// <returns>True if thread was deleted</returns>
        /// <response code="200">If the thread was created successfully</response>
        [HttpDelete("delete"), ProducesResponseType(200)]
        public ActionResult<bool> DeletePost([FromQuery]int postid = -1, [FromQuery]string pass = "")
        {
            var thread = this.Database.Threads.FirstOrDefault(x => x.Id == postid);
            if (thread != null)
            {
                var passwd = this.Database.Passwords.FirstOrDefault(x => x.Id == thread.PasswordId);
                if (passwd != null)
                {
                    var passcorrect = Passworder.VerifyPassword(pass, passwd.Hash, passwd.Salt, this.Config.DefaultPassword);
                    if (passcorrect)
                    {
                        this.Database.Threads.Remove(thread);
                        this.Database.SaveChanges();
                        return Ok();
                    }
                }

                // failed, trying with master password
                var mpasswd = this.Database.Passwords.First(x => x.Id == -1);
                var mpasscorrect = Passworder.VerifyPassword(pass, mpasswd.Hash, mpasswd.Salt, this.Config.DefaultPassword);

                if (mpasscorrect)
                {
                    this.Database.Threads.Remove(thread);
                    this.Database.SaveChanges();
                    return Ok();
                }

                return BadRequest($"Received wrong password: {pass}");
            }
            else return NotFound($"Thread with the ID '{postid}' was not found");
        }
    }
}