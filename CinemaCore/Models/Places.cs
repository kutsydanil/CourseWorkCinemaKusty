using System;
using System.Collections.Generic;

namespace CinemaCore.Models;

public partial class Places
{
    public int Id { get; set; }

    public int ListEventId { get; set; }

    public int CinemaHallId { get; set; }

    public int PlaceNumber { get; set; }

    public bool TakenSeat { get; set; }

    public virtual CinemaHalls CinemaHall { get; set; } = null!;

    public virtual ListEvents ListEvent { get; set; } = null!;
}
