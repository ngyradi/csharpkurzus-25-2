namespace Todo.Core
{
    public record class TodoItem
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsDone { get; init; }
    }
}
