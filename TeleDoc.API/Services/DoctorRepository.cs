using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeleDoc.API.Area.Doctors.Models;
using TeleDoc.API.Context;
using TeleDoc.API.Dtos.DoctorsDto;
using TeleDoc.API.Static;
using TeleDoc.DAL.Entities;

namespace TeleDoc.API.Services;

public class DoctorRepository : IDoctorRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public DoctorRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<List<DoctorDetailsDto>?> GetDoctorListAsync()
    {
        var result = await _userManager.Users
            .Where(r => r.Role == UserRoles.Doctor)
            .ToListAsync();
        var data = _mapper.Map<List<Doctor>>(result);
        var dataToReturn = _mapper.Map<List<DoctorDetailsDto>>(data);

        return dataToReturn;
    }

    public async Task<DoctorDetailsDto> GetDoctorByEmail(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        
        var data = _mapper.Map<Doctor>(result);
        var dataToReturn = _mapper.Map<DoctorDetailsDto>(data);

        return dataToReturn;
    }

    public async Task<List<DoctorDetailsDto>?> GetDoctorByName(string name)
    {
        // var result = await _userManager.FindByNameAsync(name);
        var result = await _userManager.Users
            .Where(u => u.Role == UserRoles.Doctor && u.Name!.Contains(name))
            .ToListAsync();

        var data = _mapper.Map<List<Doctor>>(result);
        var dataToReturn = _mapper.Map<List<DoctorDetailsDto>>(data);
        
        return dataToReturn;
    }

    public Task<Doctor> GetDoctorBySpeciality(string speciality)
    {
        throw new NotImplementedException();
    }

    public async Task<Doctor> UpdateDoctorByEmail(Doctor doctor)
    {
        var doc = _dbContext.Users.FirstOrDefault(d => d.Email == doctor.Email);
        // doc = _mapper.Map<Doctor>(doc);
        // doc = doctor;

        if (doc is not null)
        {
            // _dbContext.Update(doc);
            doc.Name = doctor.Name;
            doc.Speciality = doctor.Speciality;
            doc.Gender = doctor.Gender;
            doc.Address = doctor.Address;
            doc.College = doctor.College;
            doc.DateOfBirth = doctor.DateOfBirth;

            // doc = doctor;
        }

        await _dbContext.SaveChangesAsync();

        return doctor;
    }
}