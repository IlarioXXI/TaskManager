using AutoMapper;
using TaskManager.Models;
using TaskManagerWeb.Api.Models;
using TaskManagerWeb.Api.ViewModels;
using TaskManagerWEB.Api.ViewModels;
using TaskManagerWEB.Api.ViewModels.UserViewModels;

namespace TaskManagerWEB.Api.Configuration
{
    public class AutoMapperConfiguration : Profile
    {
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<TaskItem, TaskItemVM>()
                    .ForMember(vm => vm.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                    .ForMember(vm => vm.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                    .ForMember(vm => vm.PriorityName, opt => opt.MapFrom(src => src.Priority.Name))
                    .ReverseMap();
                CreateMap<Comment, CommentVM>()
                    .ForMember(vm => vm.TaskItemTitle, opt => opt.MapFrom(src => src.TaskItem.Title))
                    .ForMember(vm => vm.AppUserEmail, opt => opt.MapFrom(src => src.AppUser.Email))
                    .ReverseMap();
                CreateMap<AppUser,AppUserVM>()
                    .ForMember(vm=>vm.Email,opt => opt.MapFrom(src=>src.Email))
                    .ReverseMap();
                CreateMap<History, HistoryVM>()
                    .ForMember(vm=>vm.TaskItemName, opt => opt.MapFrom(src => src.TaskItem.Title))
                    .ForMember(vm => vm.AppUserEmail, opt => opt.MapFrom(src => src.AppUser.Email))
                    .ReverseMap();
                CreateMap<Team,TeamVM>()
                    .ForMember(vm=>vm.Users, opt => opt.MapFrom(src => src.Users))
                    .ForMember(vm => vm.TaskItems , opt=> opt.MapFrom(src=>src.TaskItems))
                    .ForMember(vm => vm.UsersIds, opt => opt.MapFrom(src => src.Users.Select(u => u.Id).ToList()))
                    .ForMember(vm => vm.taskItemsIds, opt => opt.MapFrom(src => src.TaskItems.Select(t => t.Id).ToList()))
                    .ForMember(vm => vm.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(vm => vm.Id, opt => opt.MapFrom(src => src.Id))
                    .ReverseMap();
            }
        }
    }
}
