namespace OpsRescue.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsersCount { get; set; }
        public int AdminCount { get; set; }
        public int OperationsManagerCount { get; set; }
        public int OperationsUserCount { get; set; }
        public int DeliveryPartnerCount { get; set; }
        public List<RecentUserDto> RecentUsers { get; set; } = new();
    }

    public class RecentUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class OperationsManagerDashboardDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string BranchLocation { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int TotalOperationsUsersInBranch { get; set; }
        public int TotalDeliveryPartnersInLocation { get; set; }
        public List<TeamMemberSummaryDto> BranchTeamMembers { get; set; } = new();
    }

    public class OperationsUserDashboardDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string BranchLocation { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int BranchColleaguesCount { get; set; }
        public List<string> BranchManagers { get; set; } = new();
    }

    public class DeliveryPartnerDashboardDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PartnerId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string OperatingLocation { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public int PeersInOperatingLocation { get; set; }
    }

    public class TeamMemberSummaryDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
    }
}
