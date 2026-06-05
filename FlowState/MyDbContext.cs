using FlowState.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace FlowState
{
    public class MyDbContext : DbContext
    {
        
        public DbSet<ToDoTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<SessionInvite> SessionInvites { get; set; }

        public DbSet<SessionUser> SessionUsers { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoTask>()
                .HasIndex(t => t.GoogleId)
                .IsUnique()
                .HasFilter("[GoogleId] IS NOT NULL");
        }
    }
}
