using CarRental.Common.Enums;
using CarRental.Models.Auth;
using CarRental.Models.User;
using CarRental.Repositories.DBContext;
using CarRental.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CarRental.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IUploadService _uploadService;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public UserRepository(AppDbContext context, IUploadService uploadService, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _uploadService = uploadService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Entities.User>> GetAll()
        {
            var users = await _context.Users.Include(x => x.ApprovalApplication).ToListAsync();

            return users;
        }

        public async Task<Entities.User> GetById(long id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.PostVehicles)
                .Include(x => x.FollowVehicles)
                .ThenInclude(x => x.PostVehicle)
                .Include(x => x.UserRentVehicles)
                .ThenInclude(x => x.PostVehicle)
                .Include(x => x.UserReviewVehicles)
                .ThenInclude(x => x.PostVehicle)
                .Include(x => x.ApprovalApplication)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<Entities.User> Login(LoginVM request)
        {
            var u = await _context.Users.Include(x => x.ApprovalApplication).FirstOrDefaultAsync(x => x.Email == request.Email);
            if (u == null)
                return null;
            if (!u.Status)
                return null;
            if (u.Password == Encrypt(request.Password))
            {
                return u;
            }
            return null;
        }

        private static string Encrypt(string text)
        {
            using var md5 = new MD5CryptoServiceProvider();
            using var tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            using (var transform = tdes.CreateEncryptor())
            {
                byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                return Convert.ToBase64String(bytes, 0, bytes.Length);
            }
        }

        public async Task<bool> Register(RegisterVM request)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (u != null)
                return false;

            var user = new Entities.User()
            {
                Email = request.Email,
                Name = request.Name,
                Password = Encrypt(request.Password),
                Role = ROLE_TYPE.USER,
                Avatar = "/user-content/default-user.png",
            };

            await _context.Users.AddAsync(user);

            var res = await _context.SaveChangesAsync() > 0;

            //_mailService.SendMail(new CreateMailRequest()
            //{
            //	Content = "Bạn đã đăng ký tài khoản thành công với email: " + request.Email,
            //	Title = "Đăng ký thành công",
            //	Email = request.Email,
            //	Name = request.Name,
            //});
            return res;
        }

        public async Task<bool> Toggle(long id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            u.Status = !u.Status;
            _context.Users.Update(u);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task EditInfoUser(UserEditVM vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == vm.Id);

            var avatar = user.Avatar;
            user.Name = vm.Name;
            user.Address = vm.Address;
            user.Phone = vm.Phone;

            if (vm.Image != null)
            {
                user.Avatar = await _uploadService.SaveFile(vm.Image);
                await _uploadService.DeleteFile(avatar);
            }
            _context.Users.Update(user);
            var success = await _context.SaveChangesAsync() > 0;
            _httpContextAccessor.HttpContext.Session.SetString("Name", user.Name);
            _httpContextAccessor.HttpContext.Session.SetString("Avatar", user.Avatar);
            if (success && vm.Image != null)
                await _uploadService.DeleteFile(avatar);
        }

        public async Task<bool> ChangePasswordUser(ChangePasswordVM vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (user.Password != Encrypt(vm.OldPassword))
            {
                return false;
            }

            user.Password = Encrypt(vm.NewPassword);

            _context.Users.Update(user);

            var success = await _context.SaveChangesAsync() > 0;

            return success;
        }

        public async Task<bool> RequestApprove(long id)
        {
            var u = await _context.ApprovalApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            u.IsApprove = true;
            u.User.Role = ROLE_TYPE.OWNER;

            _context.ApprovalApplications.Update(u);

            var res = await _context.SaveChangesAsync() > 0;

            _httpContextAccessor.HttpContext.Session.SetString("IsApproved", true.ToString());
            return res;
        }

        public async Task CreateApprovalApplication(ApprovalApplicationVM vm)
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return;
            var app = new Entities.ApprovalApplication()
            {
                UserId = userId,
                Address = vm.Address,
                Description = vm.Description,
                Email = vm.Email,
                Identity = vm.Identity,
                Name = vm.Name,
                Title = vm.Title,
                Phone = vm.Phone,
                Type = vm.Type,
                IsApprove = false,
            };

            await _context.ApprovalApplications.AddAsync(app);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RequestEvict(long id)
        {
            var u = await _context.ApprovalApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);


            _context.ApprovalApplications.Remove(u);

            _httpContextAccessor.HttpContext.Session.SetString("IsApproved", false.ToString());
            _httpContextAccessor.HttpContext.Session.SetString("IsApproving", false.ToString());
            return await _context.SaveChangesAsync() > 0;
        }
    }
}