using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess;
using KatlaSport.DataAccess.ProductStoreHive;
using DbHiveSection = KatlaSport.DataAccess.ProductStoreHive.StoreHiveSection;

namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Represents a hive section service.
    /// </summary>
    public class HiveSectionService : IHiveSectionService
    {
        private readonly IProductStoreHiveContext _context;
        private readonly IUserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiveSectionService"/> class with specified <see cref="IProductStoreHiveContext"/> and <see cref="IUserContext"/>.
        /// </summary>
        /// <param name="context">A <see cref="IProductStoreHiveContext"/>.</param>
        /// <param name="userContext">A <see cref="IUserContext"/>.</param>
        public HiveSectionService(IProductStoreHiveContext context, IUserContext userContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync()
        {
            var dbHiveSections = await _context.Sections.OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <inheritdoc/>
        public async Task<HiveSection> GetHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSections = await _context.Sections.Where(s => s.Id == hiveSectionId).ToArrayAsync();
            if (dbHiveSections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            return Mapper.Map<DbHiveSection, HiveSection>(dbHiveSections[0]);
        }

        /// <inheritdoc/>
        public async Task<List<HiveSectionListItem>> GetHiveSectionsAsync(int hiveId)
        {
            var dbHiveSections = await _context.Sections.Where(s => s.StoreHiveId == hiveId).OrderBy(s => s.Id).ToArrayAsync();
            var hiveSections = dbHiveSections.Select(s => Mapper.Map<HiveSectionListItem>(s)).ToList();
            return hiveSections;
        }

        /// <summary>
        /// Creates the hive section asynchronous.
        /// </summary>
        /// <param name="createRequest">The create request.</param>
        /// <returns>
        /// HiveSection
        /// </returns>
        /// <exception cref="RequestedResourceHasConflictException">code</exception>
        public async Task<HiveSection> CreateHiveSectionAsync(UpdateHiveSectionRequest createRequest)
        {
            var dbHiveSections = await _context.Sections.Where(h => h.Code == createRequest.Code).ToArrayAsync();
            if (dbHiveSections.Length > 0)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            var dbHive = Mapper.Map<UpdateHiveSectionRequest, DbHiveSection>(createRequest);
            dbHive.CreatedBy = _userContext.UserId;
            dbHive.LastUpdatedBy = _userContext.UserId;
            _context.Sections.Add(dbHive);

            await _context.SaveChangesAsync();

            return Mapper.Map<HiveSection>(dbHive);
        }

        /// <summary>
        /// Updates the hive section asynchronous.
        /// </summary>
        /// <param name="hiveSectionId">The hive section identifier.</param>
        /// <param name="updateRequest">The update request.</param>
        /// <returns>HiveSection</returns>
        /// <exception cref="RequestedResourceHasConflictException">code</exception>
        /// <exception cref="RequestedResourceNotFoundException">404</exception>
        public async Task<HiveSection> UpdateHiveSectionAsync(int hiveSectionId, UpdateHiveSectionRequest updateRequest)
        {
            var dbHiveSections = await _context.Sections.Where(hs => hs.Code == updateRequest.Code && hs.Id != hiveSectionId).ToArrayAsync();
            if (dbHiveSections.Length > 0)
            {
                throw new RequestedResourceHasConflictException("code");
            }

            dbHiveSections = await _context.Sections.Where(hs => hs.Id == hiveSectionId).ToArrayAsync();
            var dbHiveSection = dbHiveSections.FirstOrDefault();
            if (dbHiveSection == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            Mapper.Map(updateRequest, dbHiveSection);
            dbHiveSection.LastUpdatedBy = _userContext.UserId;

            await _context.SaveChangesAsync();

            dbHiveSections = await _context.Sections.Where(hs => hs.Id == hiveSectionId).ToArrayAsync();
            return dbHiveSections.Select(hs => Mapper.Map<HiveSection>(hs)).FirstOrDefault();
        }

        /// <summary>
        /// Deletes the hive section asynchronous.
        /// </summary>
        /// <param name="hiveSectionId">The hive section identifier.</param>
        /// <returns>void</returns>
        /// <exception cref="RequestedResourceNotFoundException">404</exception>
        /// <exception cref="RequestedResourceHasConflictException">402</exception>
        public async Task DeleteHiveSectionAsync(int hiveSectionId)
        {
            var dbHiveSections = await _context.Sections.Where(hs => hiveSectionId == hs.Id).ToArrayAsync();

            if (dbHiveSections.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbHiveSection = dbHiveSections[0];
            if (dbHiveSection.IsDeleted == false)
            {
                throw new RequestedResourceHasConflictException();
            }

            _context.Sections.Remove(dbHiveSection);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task SetStatusAsync(int hiveSectionId, bool deletedStatus)
        {
            var dbHiveSection = await _context.Sections.Where(hs => hiveSectionId == hs.Id).ToArrayAsync();

            if (dbHiveSection.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbCategory = dbHiveSection[0];
            if (dbCategory.IsDeleted != deletedStatus)
            {
                dbCategory.IsDeleted = deletedStatus;
                dbCategory.LastUpdated = DateTime.UtcNow;
                dbCategory.LastUpdatedBy = _userContext.UserId;

                await _context.SaveChangesAsync();
            }
        }
    }
}
