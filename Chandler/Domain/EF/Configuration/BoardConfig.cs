using Domain.EF.Entities.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Domain.EF.Configuration
{
    public class BoardConfig : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            var debugdata = new List<Board>()
            {
                new Board()
                {
                    Id = Guid.NewGuid(),
                    Name = "CHANdler",
                    Tag = "c",
                    Description = "CHANdler test board",
                    ImageUrl = "/res/logo.jpg"
                },
                new Board()
                {
                    Id = Guid.NewGuid(),
                    Name = "Random",
                    Tag = "r",
                    Description = "Random shit",
                },
                new Board()
                {
                    Id = Guid.NewGuid(),
                    Name = "Memes",
                    Tag = "m",
                    ImageUrl = "/res/pepo.gif",
                    Description = "haha cool and good dank memes",
                },
                new Board()
                {
                    Id = Guid.NewGuid(),
                    Name = "Meta",
                    Tag = "meta",
                    ImageUrl = "/res/wrench.png",
                    Description = "About CHANdler itself, e.g. development talk.",
                }
            };
            foreach (var item in debugdata) builder.HasData(item);
        }
    }
}