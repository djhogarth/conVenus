

namespace API.Interfaces
{
  public interface IUnitOfWork
  {
      // returns instances of our repsoitories
       IUserRepository UserRepository { get;}
       IMessageRepository MessageRepository { get;}

       ILikesRepository LikesRepository { get;}

       IPhotoRepository PhotoRepository { get; }

      // A Method to save all changes for all repositories.
       Task<bool> Complete();

      // Check if entity framework has any new changed tracked.
       bool HasChanges();


  }
}
