using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Interface.Manager;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Repository;
using AppointmentManagementSystem.ViewModels;
using System.Numerics;
using AppointmentManagementSystem.Common;

namespace AppointmentManagementSystem.Manager
{
    public class DoctorManager : CommonManager<Doctor>, IDoctorManager
    {
        private readonly ApplicationDbContext _db;

        public DoctorManager(ApplicationDbContext db):base(new CommonRepository<Doctor>(db))
        {
            _db = db;
        }
        public ICollection<Doctor> GetAll()
        {
            return Get(c => true && c.IsActive);
        }

        public ICollection<Doctor> GetAllActiveData()
        {
            return Get(c => c.IsActive);
        }

        public List<DoctorVm> GetAllActiveDataWithDesignationName()
        {
            var doctorList = (from doctor in _db.Doctors
                join designation in _db.Designations on doctor.DesignationId equals designation.Id
                where doctor.IsActive
                select new DoctorVm()
                {
                    Id = doctor.Id,
                    Name = doctor.Name,
                    Image = doctor.Image,
                    Mobile = doctor.Mobile,
                    Email = doctor.Email,
                    DOB = doctor.DOB,
                    Gender = doctor.Gender,
                    DesignationId = doctor.DesignationId,
                    BMDCNo = doctor.BMDCNo,
                    DesignationName = designation.Name,
                    Specialty = doctor.Specialty,
                    Language = doctor.Language,
                    Institute = doctor.Institute,
                    Description = doctor.Description

                }).ToList();

            return doctorList;
        }

        public Doctor GetById(string id)
        {
          return GetFirstOrDefault(c => c.Id == id);
        }

        public DoctorDetailsVm GetByIdWithDoctorDetails(string doctorId)
        {
            var doctorDetailsList = _db.Doctors
                .Join(_db.DoctorQualifications, d => d.Id, dq => dq.DoctorId, (d, dq) => new { Doctor = d, Qualification = dq })
                .Where(joined => joined.Doctor.Id == doctorId)
                .AsEnumerable()
                .GroupBy(joined => joined.Doctor)
                .Select(grouped => new DoctorDetailsVm()
                {
                    DoctorDegreeName = string.Join(", ", grouped.Select(q => $"{q.Qualification.DegreeName} ({q.Qualification.Country})")),
                    DoctorId = grouped.Key.Id,
                    DoctorImage = Utility.PathToBase64(grouped.Key.Image),
                    DoctorInstitute = grouped.Key.Institute,
                    DoctorLanguage = grouped.Key.Language,
                    DoctorName = grouped.Key.Name,
                    DoctorSpecialty = grouped.Key.Specialty,
                    DoctorDescription = grouped.Key.Description
                })
                .FirstOrDefault();

            return doctorDetailsList;
        }
    }
}
