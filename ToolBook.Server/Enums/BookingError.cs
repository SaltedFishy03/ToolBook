namespace ToolBook.Server.Enums;

public enum BookingError
{
    None,
    InvalidDate,
    BookingNotFound,
    BookingNotEditable,
    ToolNotFound,
    ToolUnavailable,
    Overlap,
    UserNotFound
}