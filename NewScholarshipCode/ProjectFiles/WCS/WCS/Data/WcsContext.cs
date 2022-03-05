using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WCS.Models;

namespace WCS.Data
{
    public class WcsContext : IdentityDbContext<User>
    {
        public new DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StudentForm> StudentForms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<FormResponse> FormResponses { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormCriterion> FormCriteria { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormRequirement> FormRequirements { get; set; }
        public DbSet<FormRating> FormRatings { get; set; }
        public DbSet<AwardCycle> AwardCycles { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<ScholarshipFund> ScholarshipFunds { get; set; }
        public DbSet<ScholarshipAward> ScholarshipAwards { get; set; }
        public DbSet<ScholarshipAwardMoney> ScholarshipAwardMonies { get; set; }
        public DbSet<StudentRating> StudentRatings { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<RecycledItem> RecycleBin { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public WcsContext(DbContextOptions<WcsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
