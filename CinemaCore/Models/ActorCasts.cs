using System;
using System.Collections.Generic;

namespace CinemaCore.Models;

public partial class ActorCasts
{
    public int Id { get; set; }

    public int ActorId { get; set; }

    public int FilmId { get; set; }

    public virtual Actors Actor { get; set; } = null!;

    public virtual Films Film { get; set; } = null!;
}
