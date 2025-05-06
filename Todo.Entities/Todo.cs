using System;
using System.Collections.Generic;

namespace Todo.Entities;

public partial class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? FinishedDate { get; set; }

    public bool IsFinished { get; set; }
}
