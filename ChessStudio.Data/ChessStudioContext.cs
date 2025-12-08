using Microsoft.EntityFrameworkCore;
using ChessStudio.Data.Models;

namespace ChessStudio.Data
{
    public class ChessStudioContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Match> Matches { get; set; }

        public ChessStudioContext(DbContextOptions<ChessStudioContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Player
            modelBuilder.Entity<Player>()
                .HasKey(p => p.PlayerId);

            modelBuilder.Entity<Player>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure Tournament
            modelBuilder.Entity<Tournament>()
                .HasKey(t => t.TournamentId);

            modelBuilder.Entity<Tournament>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Configure TournamentPlayer
            modelBuilder.Entity<TournamentPlayer>()
                .HasKey(tp => tp.TournamentPlayerId);

            modelBuilder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Tournament)
                .WithMany(t => t.TournamentPlayers)
                .HasForeignKey(tp => tp.TournamentId);

            modelBuilder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany(p => p.TournamentPlayers)
                .HasForeignKey(tp => tp.PlayerId);

            // Configure Round
            modelBuilder.Entity<Round>()
                .HasKey(r => r.RoundId);

            modelBuilder.Entity<Round>()
                .HasOne(r => r.Tournament)
                .WithMany(t => t.Rounds)
                .HasForeignKey(r => r.TournamentId);

            // Configure Match
            modelBuilder.Entity<Match>()
                .HasKey(m => m.MatchId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Round)
                .WithMany(r => r.Matches)
                .HasForeignKey(m => m.RoundId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.WhitePlayer)
                .WithMany()
                .HasForeignKey(m => m.WhitePlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.BlackPlayer)
                .WithMany()
                .HasForeignKey(m => m.BlackPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
