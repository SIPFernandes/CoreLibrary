namespace CoreLibrary.Core.Exceptions
{
    public class EntityDoesNotExistException : Exception
    {
        public EntityDoesNotExistException() : base("The entity does not exist.")
        {
        }

        public EntityDoesNotExistException(string message) : base(message)
        {
        }

        public EntityDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
