namespace Peers.Training;

public class GroupValidationException(string message) : Exception(message);

public class GroupNotFoundException(string message) : Exception(message);

public class GroupConflictException(string message) : Exception(message);
