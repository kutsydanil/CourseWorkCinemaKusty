using System;
using System.Collections.Generic;

namespace CinemaCore.Models;

public partial class StaffCasts
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public int ListEventId { get; set; }

    public virtual ListEvents ListEvent { get; set; } = null!;

    public virtual Staffs Staff { get; set; } = null!;
}
