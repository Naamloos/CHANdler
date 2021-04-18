using Domain.EF.Entities;
using Domain.EF.Entities.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public class ThreadRepository : BaseRepository<Thread>
    {
        private ChandlerContext Context { get; }

        public ThreadRepository(ChandlerContext ctx) : base(ctx)
        {
            this.Context = ctx;
        }
    }
}
