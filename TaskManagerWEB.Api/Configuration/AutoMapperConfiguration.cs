using AutoMapper;
using SQLitePCL;
using TaskManager.DataAccess.Interfaces;
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

                CreateMap<TaskItemVM, TaskItem>()
                    .ForMember(dest => dest.Team, opt => opt.Ignore())
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.Priority, opt => opt.Ignore())
                    .ForMember(dest => dest.Comments, opt => opt.Ignore())
                    .ForMember(dest => dest.History, opt => opt.Ignore())
                    .PreserveReferences();
                CreateMap<TaskItem, TaskItemVM>()
                    .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                    .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
                    .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.Name));



                CreateMap<Comment, CommentVM>()
                    .ForMember(vm => vm.TaskItemTitle, opt => opt.MapFrom(src => src.TaskItem.Title))
                    .ForMember(vm => vm.AppUserEmail, opt => opt.MapFrom(src => src.AppUser.Email));
                CreateMap<CommentVM, Comment>()
                    .ForMember(dest => dest.TaskItem, opt => opt.Ignore())
                    .PreserveReferences();


                CreateMap<AppUser, AppUserVM>()
                    .ForMember(vm => vm.Email, opt => opt.MapFrom(src => src.Email))
                    .ReverseMap();


                CreateMap<History, HistoryVM>()
                    .ForMember(vm => vm.TaskItemName, opt => opt.MapFrom(src => src.TaskItem.Title))
                    .ForMember(vm => vm.AppUserEmail, opt => opt.MapFrom(src => src.AppUser.Email))
                    .ReverseMap();



                CreateMap<Team, TeamVM>()
                    .ForMember(vm => vm.Users, opt => opt.MapFrom(src => src.Users))
                    .ForMember(vm => vm.TaskItems, opt => opt.MapFrom(src => src.TaskItems))
                    .ForMember(vm => vm.UsersIds, opt => opt.MapFrom(src => src.Users.Select(u => u.Id).ToList()))
                    .ForMember(vm => vm.taskItemsIds, opt => opt.MapFrom(src => src.TaskItems.Select(t => t.Id).ToList()))
                    .ForMember(vm => vm.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(vm => vm.Id, opt => opt.MapFrom(src => src.Id))
                    .ReverseMap();

                CreateMap<TeamVM, Team>()
                    .ForMember(dest => dest.TaskItems, opt => opt.MapFrom(vm => vm.taskItemsIds.Select(id => new TaskItem { Id = id }).ToList()))
                    .ForMember(dest => dest.Users, opt => opt.MapFrom(vm => vm.UsersIds.Select(id => new AppUser { Id = id }).ToList()));
            }
        }
    }
}
