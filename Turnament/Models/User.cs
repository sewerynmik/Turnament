using System.ComponentModel.DataAnnotations;

namespace Turnament.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PassHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public ICollection<TeamInvitation> ReceivedInvitations { get; set; } = new List<TeamInvitation>();
    public ICollection<TeamInvitation> SentInvitations { get; set; } = new List<TeamInvitation>();
    public ICollection<Tournament> CreatedTournaments { get; set; } = new List<Tournament>();
    public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
}

/*
Table Users {
  id int [pk, increment]
  username varchar(50) [unique, not null]
  email varchar(100) [unique, not null]
  password_hash varchar(255) [not null]
  created_at datetime [default: `CURRENT_TIMESTAMP`]
}

Table Sports {
  id int [pk, increment]
  name varchar(100) [not null, unique]
}

Table BracketTypes {
  id int [pk, increment]
  name varchar(50) [not null, unique] // np. "Single Elimination", "Double Elimination", "Round Robin"
}

Table Tournaments {
  id int [pk, increment]
  name varchar(100) [not null]
  description text
  sport_id int [not null, ref: > Sports.id]
  bracket_type_id int [not null, ref: > BracketTypes.id]
  creator_id int [not null, ref: > Users.id]
  start_date datetime
  end_date datetime
  winner_team_id int [ref: > Teams.id]
  created_at datetime [default: `CURRENT_TIMESTAMP`]
}

Table Teams {
  id int [pk, increment]
  name varchar(100) [not null, unique]
  creator_id int [not null, ref: > Users.id]
  created_at datetime [default: `CURRENT_TIMESTAMP`]
}

Table TeamMembers {
  id int [pk, increment]
  team_id int [not null, ref: > Teams.id]
  user_id int [not null, ref: > Users.id]
  role varchar(20) [default: 'member'] // np. 'member' lub 'captain'
  joined_at datetime [default: `CURRENT_TIMESTAMP`]

  indexes {
    (team_id, user_id) [unique]
  }
}

Table TeamInvitations {
  id int [pk, increment]
  team_id int [not null, ref: > Teams.id]
  invited_user_id int [not null, ref: > Users.id]
  invited_by_user_id int [not null, ref: > Users.id]
  status varchar(20) [default: 'pending'] // pending, accepted, rejected
  created_at datetime [default: `CURRENT_TIMESTAMP`]

  indexes {
    (team_id, invited_user_id) [unique]
  }
}

Table TournamentTeams {
  id int [pk, increment]
  tournament_id int [not null, ref: > Tournaments.id]
  team_id int [not null, ref: > Teams.id]

  indexes {
    (tournament_id, team_id) [unique]
  }
}

Table Matches {
  id int [pk, increment]
  tournament_id int [not null, ref: > Tournaments.id]
  round int [not null] // np. 1 = ćwierćfinały, 2 = półfinały itd.
  team1_id int [ref: > Teams.id]
  team2_id int [ref: > Teams.id]
  winner_id int [ref: > Teams.id]
  scheduled_at datetime
  finished_at datetime
}

*/