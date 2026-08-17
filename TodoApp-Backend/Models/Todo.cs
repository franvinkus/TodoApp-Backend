using System;
using System.Collections.Generic;

namespace TodoApp_Backend.Models;

public partial class Todo
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Users User { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? FinishedDate { get; set; }
    public bool IsFinished { get; set; }

}
