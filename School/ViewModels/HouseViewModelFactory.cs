using ACM.Helpers.EmailServiceFactory;
using ACM.Models.SystemModelFactory;
using Microsoft.AspNetCore.Mvc.Rendering;
using School.Models.HouseModelFactory;
using System.ComponentModel.DataAnnotations;
namespace ACM.ViewModels.HouseViewModelFactory
{
    public class HousesViewModel
    {
        internal AppDBContext _context;
        internal SecurityOptions _securityOptions;
        internal IEmailService _emailService;
        internal ClaimsPrincipal _user;
        internal JwtIssuerOptions _jwtOptions;

        internal string errorMessage = "";

        public Guid HouseID { get; set; }
        public Guid HouseImageID { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
		public string HouseImage { get; set; }
		public string Address { get; set; }
		public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string HouseImageName { get; set; }        
        public Guid? UserID { get; set; }
        public List<HouseImageListData> Images { get; set; }

		internal async Task PopulateDetails()
        {
            if (HouseID != Guid.Empty)
            {
                var item = _context.Houses.Where(x => x.HouseID == HouseID).FirstOrDefault();
                if (item != null)
                {
                    Description = item.Description;
                    Address = item.Address;
                    Location = item.Location;
                    HouseImage = item.HouseImage;
                }

                var list = (from t in _context.HouseImages
                            where t.HouseID == HouseID
                            select new HouseImageListData
                            {
                                HouseImageID = t.HouseImageID,
                                HouseImage = t.Image
                            });
                Images = list.ToList();
			}
        }

        internal async Task<Guid> Save()
        {
            bool isNew = false;
            bool isValid = true;
            errorMessage = "";
            //Validate inputs
            if (isValid)
            {
                var house = _context.Houses.Where(x => x.HouseID == HouseID).FirstOrDefault();
                if (house == null)
                {
					house = new House();
                    isNew = true;
					house.HouseID = Guid.NewGuid();
					house.CreatedDateTime = DateTime.Now;
				}
				house.Description = Description;
                house.HouseImage = HouseImage;
                house.UserID = UserID;
                house.Location = Location;
                house.Latitude = Latitude;
                house.Longitude = Longitude;
                house.Address = Address;
                house.EditDateTime = DateTime.Now;


                if (isNew)
                {
                    _context.Add(house);
                }
                else
                {
                    _context.Update(house);
                }

                
                foreach (var img in Images)
                {
					var image = new HouseImage();
                    image.HouseImageID = Guid.NewGuid();
                    image.HouseID = house.HouseID;
                    image.HouseImageName = HouseImageName;
                    image.Image = img.HouseImage;

                    _context.HouseImages.Add(image);
				}

                await _context.SaveChangesAsync();
                HouseID = house.HouseID;
            }
            return HouseID;
        }
        internal async Task<bool> Remove()
        {
            bool returnValue = false;

            var item = _context.Houses.FirstOrDefault(x => x.HouseID == HouseID);
            if (item != null)
            {
                var metaFiledsRem = _context.HouseImages.Where(x => x.HouseID == item.HouseID).ToList();
                _context.RemoveRange(metaFiledsRem);

                _context.Remove(item);

                await _context.SaveChangesAsync();
                returnValue = true;
            }
            return returnValue;
        }
        internal async Task<bool> RemoveImage()
		{
            bool returnValue = false;

            var houseImage = _context.HouseImages.FirstOrDefault(x => x.HouseImageID == HouseImageID);
            if (houseImage != null)
            {
                _context.HouseImages.Remove(houseImage);
                await _context.SaveChangesAsync();
				returnValue = true;
			}
			return returnValue;
		}
    }
    public class HousesListViewModel
    {
        internal AppDBContext _context;
        internal SecurityOptions _securityOptions;
        internal IEmailService _emailService;
		internal ClaimsPrincipal _user;
        public string SearchValue { get; set; }
        public PaginationViewModel Pagination { get; set; }
        public List<HouseListViewModelData> HousesList { get; set; }
        internal async Task PopulateLists()
        {
            if (Pagination == null)
            {
                Pagination = new PaginationViewModel();
                Pagination.Top = 10;
            }
            var list = (from t in _context.Houses
                        where (!string.IsNullOrEmpty(SearchValue) && (t.Description.Contains(SearchValue)) || string.IsNullOrEmpty(SearchValue))
                        orderby t.Description
                        select new HouseListViewModelData
                        {
                            HouseID = t.HouseID,
                            Description = t.Description,
                            Address = t.Address,
                            Location = t.Location,
                            Latitude= t.Latitude,
                            Longitude = t.Longitude,
                            HouseImage = t.HouseImage
                        });

            Pagination.TotalRecords = list.Count();
            if (!string.IsNullOrEmpty(Pagination.SortBy))
            {
                list = list.OrderByName(Pagination.SortBy, Pagination.Descending);
            }
            HousesList = list.Skip(Pagination.Skip).Take(Pagination.Top).ToList();
        }
    }
    public class HouseListViewModelData
    {
        public Guid HouseID { get; set; }
        public string Description { get; set; }
		public string HouseImage { get; set; }
		public string Address { get; set; }
		public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public List<HouseImageListData> Images { get; set; }
	}
	public class HouseImageListData
    {
        public Guid HouseImageID { get; set; }
        public string HouseImageName { get; set; }
        public string HouseImage { get; set; }
	}
}
