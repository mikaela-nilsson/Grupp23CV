using Grupp23_CV.Database;
using Grupp23_CV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Grupp23_CV.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationUserDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(ApplicationUserDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateMessage(int receiverId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            string? senderName = currentUser != null
                ? (await _context.CVs.FirstOrDefaultAsync(c => c.UserId == currentUser.Id))?.FullName ?? currentUser.UserName
                : null;

            var model = new Message
            {
                ReceiverId = receiverId,
                SenderId = currentUser?.Id,
                SenderName = senderName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!User.Identity.IsAuthenticated && string.IsNullOrWhiteSpace(model.SenderName))
            {
                ModelState.AddModelError(nameof(model.SenderName), "Ett avsändarnamn krävs om du inte är inloggad.");

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            string senderName;

            if (user != null)
            {
                var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == user.Id);
                senderName = cv?.FullName ?? user.UserName;
            }
            else
            {
                senderName = model.SenderName;
            }

            var message = new Message
            {
                SenderId = user?.Id,
                SenderName = senderName,
                ReceiverId = model.ReceiverId,
                Content = model.Content,
                SentDate = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Meddelandet skickades!";
            ModelState.Clear();
            model.Content = string.Empty;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReceivedMessages()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var unreadMessages = await _context.Messages
            .Where(m => m.ReceiverId == currentUser.Id && !m.IsRead)
            .ToListAsync();

            return View(unreadMessages); //Skicka olästa meddelanden till view:en
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveChanges(List<int> IsRead)
        {
            if (IsRead == null || !IsRead.Any())
            {
                TempData["ErrorMessage"] = "Inga meddelanden har valts.";
                return RedirectToAction("ReceivedMessages");
            }

            var messages = await _context.Messages
                .Where(m => IsRead.Contains(m.Id))
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            _context.UpdateRange(messages);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ändringarna sparades!";

            return RedirectToAction("ReceivedMessages");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReadMessages()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var readMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id && m.IsRead)
                .ToListAsync();

            return View(readMessages);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ConfirmDelete(List<int> SelectedMessages)
        {
            if (SelectedMessages == null || !SelectedMessages.Any())
            {
                TempData["ErrorMessage"] = "Inga meddelanden har valts.";
                return RedirectToAction("ReadMessages");
            }

            Console.WriteLine("Valda meddelanden: " + string.Join(", ", SelectedMessages));

            return View(SelectedMessages); //Skickar de valda meddelandena till bekräftelsevyn
        }

        [HttpPost]
        [Authorize]
        public IActionResult CancelDelete()
        {
            TempData["ErrorMessage"] = "Raderingen avbröts.";
            return RedirectToAction("ReadMessages");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteMessages(List<int> SelectedMessages)
        {
            var messagesToDelete = await _context.Messages
                .Where(m => SelectedMessages.Contains(m.Id))
                .ToListAsync();

            _context.Messages.RemoveRange(messagesToDelete);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "De valda meddelandena har tagits bort.";

            return RedirectToAction("ReadMessages");
        }
    }
}
