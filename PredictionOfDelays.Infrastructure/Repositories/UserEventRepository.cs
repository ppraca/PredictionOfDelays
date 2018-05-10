﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PredictionOfDelays.Core.Models;
using PredictionOfDelays.Core.Repositories;
using PredictionOfDelays.Infrastructure.Utilities;

namespace PredictionOfDelays.Infrastructure.Repositories
{
    public class UserEventRepository : IUserEventRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<RepositoryActionResult<UserEvent>> AddAsync(UserEvent userEvent)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == userEvent.EventId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userEvent.ApplicationUserId);

            if (@event == null || user == null)
            {
                return new RepositoryActionResult<UserEvent>(userEvent, RepositoryStatus.NotFound);
            }

            try
            {
                _context.UserEvents.Add(userEvent);
                await _context.SaveChangesAsync();
                return new RepositoryActionResult<UserEvent>(userEvent, RepositoryStatus.Created);
            }
            catch (Exception exception)
            {
                return new RepositoryActionResult<UserEvent>(userEvent, RepositoryStatus.Error);
            }
        }

        public async Task<RepositoryActionResult<UserEvent>> RemoveAsync(UserEvent userEvent)
        {
            var ue = await _context.UserEvents.FirstOrDefaultAsync(usEv =>
                usEv.EventId == userEvent.EventId && usEv.ApplicationUserId == userEvent.ApplicationUserId);

            if (ue == null)
            {
                return new RepositoryActionResult<UserEvent>(null, RepositoryStatus.NotFound);
            }

            try
            {
                _context.UserEvents.Remove(userEvent);
                await _context.SaveChangesAsync();
                return new RepositoryActionResult<UserEvent>(userEvent, RepositoryStatus.Deleted);
            }
            catch (Exception exception)
            {
                return new RepositoryActionResult<UserEvent>(userEvent, RepositoryStatus.Error);
            }
        }

        public async Task<RepositoryActionResult<IQueryable<ApplicationUser>>> GetAttendeesAsync(int eventId)
        {
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);

            if (@event == null)
            {
                return new RepositoryActionResult<IQueryable<ApplicationUser>>(null, RepositoryStatus.NotFound);
            }

            var attendees = _context.UserEvents.Include("ApplicationUser").Where(ue => ue.EventId == eventId)
                .Select(ue => ue.ApplicationUser);

            return new RepositoryActionResult<IQueryable<ApplicationUser>>(attendees, RepositoryStatus.Ok);

        }

        public RepositoryActionResult<IQueryable<Event>> GetEvents(string userId)
        {
            var events = _context.UserEvents.Include("Event").Where(ug => ug.ApplicationUserId == userId)
                .Select(u => u.Event);

            return new RepositoryActionResult<IQueryable<Event>>(events, RepositoryStatus.Ok);
        }

        public async Task<RepositoryActionResult<EventInvite>> AddInviteAsync(EventInvite invite)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == invite.SenderId);
            var invited = await _context.Users.FirstOrDefaultAsync(u => u.Id == invite.InvitedId);
            var @event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == invite.EventId);
            if (sender == null || invited == null || @event == null)
            {
                return new RepositoryActionResult<EventInvite>(invite, RepositoryStatus.NotFound);
            }

            var existingInvite = await _context.EventInvites.FirstOrDefaultAsync(
                i => i.EventId == invite.EventId && i.InvitedId == invite.InvitedId);

            if (existingInvite != null)
            {
                return new RepositoryActionResult<EventInvite>(existingInvite, RepositoryStatus.BadRequest);
            }

            try
            {
                invite.EventInviteId = Guid.NewGuid();
                var result = _context.EventInvites.Add(invite);
                await _context.SaveChangesAsync();
                new InviteSender().SendEventInvite(invite);
                return new RepositoryActionResult<EventInvite>(result, RepositoryStatus.Created);
            }
            catch (Exception e)
            {
                return new RepositoryActionResult<EventInvite>(invite, RepositoryStatus.Error);
            }
        }

        public Task<RepositoryActionResult<EventInvite>> AddInviteGroupAsync(EventInvite invite)
        {
            throw new NotImplementedException();
        }

        public async Task<RepositoryActionResult<UserEvent>> AcceptInvitationAsync(Guid inviteId, string receiverId)
        {
            var eventInvite = await _context.EventInvites.FirstOrDefaultAsync(
                i => i.EventInviteId == inviteId && i.InvitedId == receiverId);

            if (eventInvite == null)
            {
                return new RepositoryActionResult<UserEvent>(null, RepositoryStatus.NotFound);
            }
            try
            {
                var entity = _context.UserEvents.Add(new UserEvent()
                {
                    ApplicationUserId = eventInvite.InvitedId,
                    EventId = eventInvite.EventId
                });

                //Clean invites
                _context.EventInvites.Remove(eventInvite);

                await _context.SaveChangesAsync();
                return new RepositoryActionResult<UserEvent>(entity, RepositoryStatus.Created);
            }
            catch (Exception)
            {
                return new RepositoryActionResult<UserEvent>(null, RepositoryStatus.Error);
            }
        }

        public async Task<RepositoryActionResult<EventInvite>> RejectInvitationAsync(Guid inviteId, string receiverId)
        {
            var eventInvite = await _context.EventInvites.FirstOrDefaultAsync(
                i => i.EventInviteId == inviteId && i.InvitedId == receiverId);

            if (eventInvite == null)
            {
                return new RepositoryActionResult<EventInvite>(null, RepositoryStatus.NotFound);
            }
            try
            {
                _context.EventInvites.Remove(eventInvite);

                await _context.SaveChangesAsync();
                return new RepositoryActionResult<EventInvite>(null, RepositoryStatus.Deleted);
            }
            catch (Exception)
            {
                return new RepositoryActionResult<EventInvite>(null, RepositoryStatus.Error);
            }
        }
    }
}