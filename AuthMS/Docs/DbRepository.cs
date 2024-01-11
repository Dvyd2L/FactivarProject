//using Helpers.Enums;
//using Microsoft.EntityFrameworkCore;
//using Services.Interfaces;

//namespace AuthMS.Docs;

//public interface IDbRepository<T> where T : class
//{
//    Task<IEnumerable<T>> GetAll();
//    Task<T?> GetById(object id);
//    Task Insert(T obj);
//    Task Update(T obj);
//    Task Delete(object id);
//    Task Save();
//}

//public class DbRepository<T>(DbContext context) 
//    : IDbRepository<T> 
//    where T : class
//{
//    private readonly DbContext _context = context;
//    private readonly DbSet<T> _dbSet = context.Set<T>();

//    public async Task<IEnumerable<T>> GetAll()
//    {
//        return await _dbSet.ToListAsync();
//    }

//    public async Task<T?> GetById(object id)
//    {
//        return await _dbSet.FindAsync(id);
//    }

//    public async Task Insert(T obj)
//    {
//        await _dbSet.AddAsync(obj);
//    }

//    public Task Update(T obj)
//    {
//        _dbSet.Attach(obj);
//        _context.Entry(obj).State = EntityState.Modified;
//        return Task.CompletedTask;
//    }

//    public async Task Delete(object id)
//    {
//        T? existing = await _dbSet.FindAsync(id);

//        if (existing is not null)
//        {
//            _dbSet.Remove(existing);
//        }
//    }


//    public async Task Save()
//    {
//        await _context.SaveChangesAsync();
//    }
//}

//public class RastroService<TContext>(
//    IDbRepository<Usuario> usuarioRepository, 
//    IDbRepository<Rastro> rastroRepository, 
//    IHttpContextAccessor accessor, 
//    IHostEnvironment hostEnvironment
//    ) : IRastroService 
//    where TContext : DbContext
//{
//    private readonly IDbRepository<Usuario> _usuarioRepository = usuarioRepository;
//    private readonly IDbRepository<Rastro> _rastroRepository = rastroRepository;
//    private readonly IHttpContextAccessor _accessor = accessor;
//    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

//    public async Task<int> AddRastro(string usuario, EnumTipoProcesoRastro proceso, EnumTipoAccionRastro operacion, string observaciones)
//    {
//        if (!_hostEnvironment.IsDevelopment())
//        {
//            var idUsuario = (from x in _usuarioRepository.GetAll()
//                             where x.Email == usuario
//                             select x.IdUsuario).SingleOrDefault();

//            Rastro rastro = new Rastro()
//            {
//                FechaAccion = DateTime.Now,
//                Observaciones = observaciones,
//                Proceso = Enum.GetName(typeof(EnumTipoProcesoRastro), proceso),
//                Operacion = Enum.GetName(typeof(EnumTipoAccionRastro), operacion),
//                Usuarios_IdUsuario = idUsuario,
//                Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
//            };

//            await _rastroRepository.Insert(rastro);
//            await _rastroRepository.Save();
//        }

//        return await Task.FromResult(0);
//    }
//}