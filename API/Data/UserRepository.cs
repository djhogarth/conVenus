namespace API.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<MemberDTO> GetMemberByUsernameAsync(string username, bool isCurrentUser)
    {
      var query = _context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
        .AsQueryable();

        if(isCurrentUser) query = query.IgnoreQueryFilters();

        return await query.FirstOrDefaultAsync();

    }
    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParameters parameters)
    {
       var query = _context.Users.AsQueryable();

      query = query.Where(u => u.UserName != parameters.CurrentUsername);
      query = query.Where(u => u.Gender == parameters.Gender);

      /* select a min and max age which filters members by date of birth */
      var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-parameters.MaxAge -1));
      var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-parameters.MinAge));

      query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

      //order members by last created or the default case, last active
      query = parameters.OrderBy switch{
        "created" => query.OrderByDescending(u => u.Created),
         _ => query.OrderByDescending(u => u.LastActive)
      };

      return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(_mapper
        .ConfigurationProvider).AsNoTracking(),
          parameters.PageNumber, parameters.PageSize);
        ;
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
     return await _context.Users
      .Include(p => p.Photos)
      .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<string> GetUserGender(string username)
    {
      return await _context.Users
        .Where(x => x.UserName == username)
        .Select(x => x.Gender).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
      return await _context.Users
      .Include(p => p.Photos)
      .ToListAsync();
    }
    public void UpdateUser(AppUser user)
    {
      /*mark the entity as modified so Entity Frameork
        can update and mark entity as modified
       */
      _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<AppUser> GetUserByPhotoId(int photoId)
    {
      return await _context.Users
        .Include(p => p.Photos)
        .IgnoreQueryFilters()
        .Where(p => p.Photos.Any(p => p.Id == photoId))
        .FirstOrDefaultAsync();
    }


  }
}
