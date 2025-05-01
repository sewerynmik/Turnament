using Microsoft.EntityFrameworkCore;
using Turnament.Models;

namespace Turnament.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<BracketType> BracketTypes { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<TeamInvitation> TeamInvitations { get; set; }
    public DbSet<TournamentTeam> TournamentTeams { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TeamMember>()
            .HasIndex(tm => new { tm.TeamId, tm.UserId })
            .IsUnique();

        modelBuilder.Entity<TeamMember>()
            .HasOne(tm => tm.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamMember>()
            .HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamInvitation>()
            .HasIndex(ti => new { ti.TeamId, ti.InvitedUserId })
            .IsUnique();

        modelBuilder.Entity<TeamInvitation>()
            .HasOne(ti => ti.Team)
            .WithMany(t => t.Invitations)
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamInvitation>()
            .HasOne(ti => ti.InvitedUser)
            .WithMany(u => u.ReceivedInvitations)
            .HasForeignKey(ti => ti.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamInvitation>()
            .HasOne(ti => ti.InvitedByUser)
            .WithMany(u => u.SentInvitations)
            .HasForeignKey(ti => ti.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TournamentTeam>()
            .HasIndex(tt => new { tt.TournamentId, tt.TeamId })
            .IsUnique();

        modelBuilder.Entity<TournamentTeam>()
            .HasOne(tt => tt.Tournament)
            .WithMany(t => t.TournamentTeams)
            .HasForeignKey(tt => tt.TournamentId);

        modelBuilder.Entity<TournamentTeam>()
            .HasOne(tt => tt.Team)
            .WithMany(t => t.TournamentTeams)
            .HasForeignKey(tt => tt.TeamId);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team1)
            .WithMany(t => t.MatchesAsTeam1)
            .HasForeignKey(m => m.Team1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team2)
            .WithMany(t => t.MatchesAsTeam2)
            .HasForeignKey(m => m.Team2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Winner)
            .WithMany(t => t.MatchesWinners)
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.Sport)
            .WithMany(s => s.Tournaments)
            .HasForeignKey(t => t.SportId);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.BracketType)
            .WithMany(b => b.Tournaments)
            .HasForeignKey(t => t.BracketTypeId);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTournaments)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.WinnerTeam)
            .WithMany()
            .HasForeignKey(t => t.WinnerTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}