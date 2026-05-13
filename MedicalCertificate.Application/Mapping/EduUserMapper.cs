using MedicalCertificate.Application.DTOs;
using MedicalCertificate.Domain.Entities;

namespace MedicalCertificate.Application.Mapping;

// TODO(copilot): temporary EDU projection helper; shrink this when the schema is narrowed to certificate-only needs.
public static class EduUserMapper
{
    public static string GetDisplayName(Edu_Users? user)
    {
        if (user is null)
        {
            return string.Empty;
        }

        var parts = new[] { user.LastName, user.FirstName, user.MiddleName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim());

        var displayName = string.Join(" ", parts).Trim();
        return string.IsNullOrWhiteSpace(displayName) ? user.Email ?? string.Empty : displayName;
    }

    public static UserDto ToUserDto(Edu_Users user)
    {
        var (department, institute) = ResolveEducationInfo(user);

        return new UserDto
        {
            Id = user.ID,
            LastName = user.LastName,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            Email = user.Email,
            PersonalEmail = user.PersonalEmail,
            DOB = user.DOB,
            PlaceOfBirth = user.PlaceOfBirth,
            Male = user.Male,
            HomePhone = user.HomePhone,
            MobilePhone = user.MobilePhone,
            IIN = user.IIN,
            PhotoFileName = user.PhotoFileName,
            PhotoFileData = user.PhotoFileData,
            LastUpdatedBy = user.LastUpdatedBy,
            LastUpdatedOn = user.LastUpdatedOn,
            FileContainerID = user.FileContainerID,
            MobilePushID = user.MobilePushID,
            oldId = user.oldId,
            ESUVOID = user.ESUVOID,
            ExtraFileContainerID = user.ExtraFileContainerID,
            Resident = user.Resident,
            Hero_Person_ID = user.Hero_Person_ID,
            IsReadTeamsNotif = user.IsReadTeamsNotif,
            NationalityID = user.NationalityID,
            MaritalStatusID = user.MaritalStatusID,
            MessengerTypeID = user.MessengerTypeID,
            CitizenshipCountryID = user.CitizenshipCountryID,
            CitizenCategoryID = user.CitizenCategoryID,
            Department = department,
            Institute = institute
        };
    }

    public static (string? Department, string? Institute) ResolveEducationInfo(Edu_Users? user)
    {
        var specialityUnit = user?.Student?.Speciality;
        var department = ResolveUnitName(specialityUnit);
        var institute = ResolveUnitName(specialityUnit?.Parent);

        var current = specialityUnit;
        while (current is not null)
        {
            var typeTitle = current.Type?.Title?.Trim();

            if (department is null && IsType(typeTitle, "Кафедра", "Department", "Chair"))
            {
                department = ResolveUnitName(current);
            }

            if (institute is null && IsType(typeTitle, "Институт", "Institute"))
            {
                institute = ResolveUnitName(current);
            }

            if (department is not null && institute is not null)
            {
                break;
            }

            current = current.Parent;
        }

        return (department, institute);
    }

    private static string? ResolveUnitName(Edu_OrgUnits? unit)
    {
        if (unit is null)
        {
            return null;
        }

        return unit.Title?.Trim()
            ?? unit.ShortTitle?.Trim()
            ?? unit.ID.ToString();
    }

    private static bool IsType(string? actualType, params string[] expectedTypes)
    {
        if (string.IsNullOrWhiteSpace(actualType))
        {
            return false;
        }

        return expectedTypes.Any(expected =>
            string.Equals(actualType.Trim(), expected, StringComparison.OrdinalIgnoreCase));
    }
}
