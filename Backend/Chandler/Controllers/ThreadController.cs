﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chandler.Data;
using Chandler.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : ControllerBase
    {
        private Database database;
        private ServerMeta meta;

        public ThreadController(Database database, ServerMeta meta)
        {
            this.database = database;
            this.meta = meta;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Thread>> GetThreads(string tag = "")
        {
            var ctx = database.GetContext();

            if (ctx.Threads.Any(x => x.BoardTag == tag && x.ParentId == -1))
            {
                return ctx.Threads.Where(x => x.BoardTag == tag && x.ParentId == -1)
                    .OrderByDescending(x => ctx.Threads.Where(y => y.ParentId == x.Id || y.Id == x.Id).Select(y => y.Id).Max())
                    .ToList();
            }
            return this.NotFound("not found");
        }

        [HttpGet("post")]
        public ActionResult<IEnumerable<Thread>> GetPosts(int thread = -1)
        {
            var ctx = database.GetContext();

            return ctx.Threads.Where(x => x.ParentId == thread).OrderBy(x => x.Id).ToList();
        }

        [HttpPost("create")]
        public ActionResult<bool> CreatePost([FromBody] Thread newpost)
        {
            var ctx = database.GetContext();

            newpost.Id = 0;

            if (string.IsNullOrEmpty(newpost.Text))
                return BadRequest("text too short");

            if (string.IsNullOrEmpty(newpost.Username))
                newpost.Username = "Anonymous";

            var passw = newpost.generatepass;
            int passid = -1;
            if (!string.IsNullOrEmpty(passw))
            {
                var salt = Passworder.GenerateSalt();
                var genpass = Passworder.GenerateHash(passw, salt);
                var newpass = new Password()
                {
                    Salt = salt,
                    Hash = genpass.hash,
                    Cycles = genpass.cycles
                };
                ctx.Passwords.Add(newpass);
                ctx.SaveChanges();
                passid = newpass.Id;
            }


            ctx = database.GetContext();

            newpost.generatepass = "";
            newpost.PasswordId = passid;

            ctx.Threads.Add(newpost);
            ctx.SaveChanges();
            return true;
        }

        [HttpDelete("delete")]
        public ActionResult DeletePost(int postid = -1, [FromBody]string pass = "")
        {
            var ctx = database.GetContext();

            if (ctx.Threads.Any(x => x.Id == postid))
            {
                var thread = ctx.Threads.First(x => x.Id == postid);
                if (ctx.Passwords.Any(x => x.Id == thread.PasswordId))
                {
                    var passwd = ctx.Passwords.First(x => x.Id == thread.PasswordId);

                    bool passcorrect = Passworder.CompareHash(pass, passwd.Salt, passwd.Hash, passwd.Cycles);
                    if (passcorrect)
                    {
                        ctx.Threads.Remove(thread);
                        ctx.SaveChanges();
                        return Ok();
                    }
                }

                // failed, trying with master password
                var mpasswd = ctx.Passwords.First(x => x.Id == -1);
                bool mpasscorrect = Passworder.CompareHash(pass, mpasswd.Salt, mpasswd.Hash, mpasswd.Cycles);

                if (mpasscorrect)
                {
                    ctx.Threads.Remove(thread);
                    ctx.SaveChanges();
                    return Ok();
                }

                return NotFound($"Received wrongpass {pass}");
            }
            return NotFound();
        }
    }
}
