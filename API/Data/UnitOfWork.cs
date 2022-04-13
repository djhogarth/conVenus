namespace API.Data
{
/* The UnitOfWork class is an one stop shop where all changes from
   the repositories are saved after entity framework has tracked them.
   It is reponsible for getting the new instance of the data context
   and passing it as a parameter to the different repositories.
   The UnitOfWork class is injected into the controllers to receive
   new transactions from the client. */
  public class UnitOfWork : IUnitOfWork

  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UnitOfWork(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public ILikesRepository LikesRepository => new LikesRepository(_context);

    public IPhotoRepository PhotoRepository => new PhotoRepository(_context);

    public async Task<bool> Complete()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
      return _context.ChangeTracker.HasChanges();
    }
  }
}
