using Domain.EF.Entities;
using Domain.EF.Entities.Main;

namespace Domain.Repositories
{
    public class BoardRepository : BaseRepository<Board>
    {
        private ChandlerContext Context { get; }

        public BoardRepository(ChandlerContext ctx) : base(ctx)
        {
            this.Context = ctx;
        }
    }
}
