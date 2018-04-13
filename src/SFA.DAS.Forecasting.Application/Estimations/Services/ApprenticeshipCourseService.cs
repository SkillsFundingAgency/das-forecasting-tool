using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.Estimations.Services
{
    public class ApprenticeshipCourseService: IApprenticeshipCourseService
    {
        private static readonly List<ApprenticeshipCourse> Courses = new List<ApprenticeshipCourse>
        {
            new ApprenticeshipCourse
            {
                Id = "34",
                FundingCap = 9000,
                Duration = 18,
                Title = "Able seafarer (deck) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "176",
                FundingCap = 9000,
                Duration = 24,
                Title = "Accident Repair Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "204",
                FundingCap = 21000,
                Duration = 36,
                Title = "Accountancy Taxation Professional - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "454-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Accounting: Accounting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "454-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Accounting: Accounting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "454-20-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Accounting: Accounting - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "460-3-3",
                FundingCap = 2000,
                Duration = 12,
                Title = " Activity Leadership: Coaching - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "460-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = " Activity Leadership: Exercise and Fitness - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "460-3-4",
                FundingCap = 2000,
                Duration = 12,
                Title = " Activity Leadership: Leadership - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "460-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Activity Leadership: Outdoors - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "17",
                FundingCap = 15000,
                Duration = 24,
                Title = " Actuarial technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "119",
                FundingCap = 3000,
                Duration = 12,
                Title = " Adult care worker - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "152",
                FundingCap = 12000,
                Duration = 22,
                Title = " Advanced butcher - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "148",
                FundingCap = 9000,
                Duration = 18,
                Title = " Advanced credit controller / debt collection specialist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "149",
                FundingCap = 27000,
                Duration = 36,
                Title = " Advanced dairy technologist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "37",
                FundingCap = 27000,
                Duration = 48,
                Title = " Aerospace engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "38",
                FundingCap = 27000,
                Duration = 48,
                Title = " Aerospace software development engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "528-3-1",
                FundingCap = 1500,
                Duration = 15,
                Title = " Agriculture: Agriculture - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "528-2-1",
                FundingCap = 3000,
                Duration = 18,
                Title = " Agriculture: Agriculture - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "114",
                FundingCap = 24000,
                Duration = 48,
                Title = " Aircraft maintenance certifying engineer - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "121",
                FundingCap = 3000,
                Duration = 12,
                Title = " Airside operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "439-3-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Animal Care: Animal Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "439-2-1",
                FundingCap = 2500,
                Duration = 24,
                Title = " Animal Care: Animal Care - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "179",
                FundingCap = 6000,
                Duration = 15,
                Title = " Animal Technologist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "180",
                FundingCap = 15000,
                Duration = 24,
                Title = " Arborist - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "133",
                FundingCap = 9000,
                Duration = 18,
                Title = " Assistant accountant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "85",
                FundingCap = 9000,
                Duration = 18,
                Title = " Assistant technical director(visual effects) - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "156",
                FundingCap = 15000,
                Duration = 18,
                Title = " Associate Ambulance Practitioner - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "128",
                FundingCap = 9000,
                Duration = 24,
                Title = " Associate project manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "87",
                FundingCap = 3000,
                Duration = 12,
                Title = " Aviation ground operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "86",
                FundingCap = 3000,
                Duration = 18,
                Title = " Aviation ground specialist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "141",
                FundingCap = 12000,
                Duration = 18,
                Title = " Aviation maintenance mechanic(military) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "123",
                FundingCap = 5000,
                Duration = 18,
                Title = " Aviation operations manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "405-3-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Aviation Operations on the Ground: Aviation Operations on the Ground - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "405-2-1",
                FundingCap = 6000,
                Duration = 18,
                Title = " Aviation Operations on the Ground: Aviation Operations on the Ground - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "177",
                FundingCap = 9000,
                Duration = 12,
                Title = " Baker - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "507-3-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Barbering: Barbering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "507-2-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Barbering: Barbering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "422-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Beauty Therapy: Beauty Therapy General - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "422-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = " Beauty Therapy: Beauty Therapy General - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "422-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = " Beauty Therapy: Beauty Therapy Make - up - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "422-2-2",
                FundingCap = 3000,
                Duration = 12,
                Title = " Beauty Therapy: Beauty Therapy Make - up - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "422-2-3",
                FundingCap = 2500,
                Duration = 12,
                Title = " Beauty Therapy: Beauty Therapy Massage - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "183",
                FundingCap = 9000,
                Duration = 24,
                Title = " Bespoke Saddler - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "72",
                FundingCap = 15000,
                Duration = 24,
                Title = " Bespoke tailor and cutter - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "76",
                FundingCap = 27000,
                Duration = 48,
                Title = " Boatbuilder - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "112",
                FundingCap = 9000,
                Duration = 12,
                Title = " Broadcast production assistant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "605-22-1",
                FundingCap = 9000,
                Duration = 12,
                Title = " Broadcast Technology Higher Apprenticeship - BBC: Broadcast Technology - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "198",
                FundingCap = 27000,
                Duration = 66,
                Title = " Building Services Design Engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "178",
                FundingCap = 12000,
                Duration = 42,
                Title = " Building Services Design Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "173",
                FundingCap = 18000,
                Duration = 36,
                Title = " Building Services Engineering Craftsperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "185",
                FundingCap = 27000,
                Duration = 48,
                Title = " Building Services Engineering Ductwork Craftsperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "192",
                FundingCap = 15000,
                Duration = 24,
                Title = " Building Services Engineering Ductwork Installer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "164",
                FundingCap = 12000,
                Duration = 24,
                Title = " Building Services Engineering Installer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "193",
                FundingCap = 18000,
                Duration = 48,
                Title = " Building Services Engineering Service and Maintenance Engineer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "543-2-2",
                FundingCap = 12000,
                Duration = 42,
                Title = " Building Services Engineering Technology and Project Management: Design Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "543-2-1",
                FundingCap = 12000,
                Duration = 42,
                Title = " Building Services Engineering Technology and Project Management: Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "194",
                FundingCap = 9000,
                Duration = 24,
                Title = " Building Services Engineering Ventilation Hygiene Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "431-3-3",
                FundingCap = 4000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Body - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "431-2-3",
                FundingCap = 9000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Body - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "431-3-2",
                FundingCap = 3500,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Electrical - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "431-2-2",
                FundingCap = 9000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Electrical - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "431-3-1",
                FundingCap = 4000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Mechanical - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "431-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Mechanical - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "431-2-4",
                FundingCap = 9000,
                Duration = 24,
                Title = " Bus and Coach Engineering and Maintenance: Mechanical and Electrical - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "136",
                FundingCap = 9000,
                Duration = 36,
                Title = " Bus and coach engineering manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "134",
                FundingCap = 18000,
                Duration = 36,
                Title = " Bus and coach engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "196",
                FundingCap = 5000,
                Duration = 18,
                Title = " Business Administrator - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "54",
                FundingCap = 9000,
                Duration = 18,
                Title = " Butcher - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "584-21-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Care Leadership and Management: General Adult Social Care - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "584-21-2",
                FundingCap = 2500,
                Duration = 12,
                Title = " Care Leadership and Management: Specialist Adult Social Care - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "582-3-4",
                FundingCap = 2000,
                Duration = 12,
                Title = " Catering and Professional Chefs: Chefs in the Licensed Hospitality Industry - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "582-3-3",
                FundingCap = 3500,
                Duration = 24,
                Title = " Catering and Professional Chefs: Craft Cuisine - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "582-2-3",
                FundingCap = 5000,
                Duration = 30,
                Title = " Catering and Professional Chefs: Craft Cuisine - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "582-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Catering and Professional Chefs: Food Production and Cooking - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "582-2-2",
                FundingCap = 2500,
                Duration = 18,
                Title = " Catering and Professional Chefs: Patisserie and Confectionery - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "582-3-2",
                FundingCap = 2500,
                Duration = 18,
                Title = " Catering and Professional Chefs: Professional Cookery - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "582-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Catering and Professional Chefs: Professional Cookery - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "41",
                FundingCap = 12000,
                Duration = 60,
                Title = " Chartered legal executive - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "55",
                FundingCap = 27000,
                Duration = 48,
                Title = " Chartered manager degree apprenticeship - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "50",
                FundingCap = 27000,
                Duration = 60,
                Title = " Chartered surveyor - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "169",
                FundingCap = 9000,
                Duration = 12,
                Title = " Chef De Partie - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "445-2-2",
                FundingCap = 2500,
                Duration = 20,
                Title = " Children and Young People's Workforce: Children and Young People's Social Care - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "445-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Children and Young People's Workforce: Children and Young People's Workforce - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "445-2-1",
                FundingCap = 2500,
                Duration = 20,
                Title = " Children and Young People's Workforce: Early Years Educator - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "445-2-3",
                FundingCap = 2500,
                Duration = 20,
                Title = " Children and Young People's Workforce: Residential Childcare - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "200",
                FundingCap = 27000,
                Duration = 66,
                Title = " Civil Engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "199",
                FundingCap = 12000,
                Duration = 36,
                Title = " Civil Engineering Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "498-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Cleaning and Environmental Support Services: Cleaning and Support Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "498-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Cleaning and Environmental Support Services: Cleaning Supervision - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "498-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Cleaning and Environmental Support Services: Local Environmental Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "498-3-3",
                FundingCap = 1500,
                Duration = 12,
                Title = " Cleaning and Environmental Support Services: Pest Management - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "561-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Coaching: Coaching Swimming - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "561-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Coaching: Coaching Tennis - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "561-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Coaching: Coaching Tennis - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "222",
                FundingCap = 9000,
                Duration = 24,
                Title = " Commercial Procurement and Supply - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "93",
                FundingCap = 9000,
                Duration = 12,
                Title = " Commis chef - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "215",
                FundingCap = 6000,
                Duration = 18,
                Title = " Community Activator Coach - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "492-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Community Arts: Community Arts Administration - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "492-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = " Community Arts: Community Arts Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "617-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Community Safety: Community Fire Safety - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "143",
                FundingCap = 9000,
                Duration = 15,
                Title = " Compliance / risk officer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "596-3-1",
                FundingCap = 3000,
                Duration = 18,
                Title = " Composite Engineering: Composite Engineering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "596-2-1",
                FundingCap = 12000,
                Duration = 18,
                Title = " Composite Engineering: Composite Engineering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "160",
                FundingCap = 27000,
                Duration = 36,
                Title = " Composites Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "522-2-3",
                FundingCap = 9000,
                Duration = 30,
                Title = " Construction Building: Decorative Finishing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "522-3-1",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Building: Decorative Finishing and Industrial Painting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "522-3-2",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Building: Maintenance Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "522-3-3",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Building: Trowel Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "522-2-1",
                FundingCap = 6000,
                Duration = 30,
                Title = " Construction Building: Trowel Occupations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "522-3-4",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Building: Wood Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "522-2-2",
                FundingCap = 9000,
                Duration = 30,
                Title = " Construction Building: Wood Occupations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "522-3-5",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Building: Woodmachining - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-1",
                FundingCap = 4000,
                Duration = 18,
                Title = " Construction Civil Engineering: Construction Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-2",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Civil Engineering: Formwork Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-3",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Civil Engineering: Highways Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-4",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Civil Engineering: Plant Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-2-1",
                FundingCap = 12000,
                Duration = 30,
                Title = " Construction Civil Engineering: Plant Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-5",
                FundingCap = 4000,
                Duration = 18,
                Title = " Construction Civil Engineering: Plant Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-7",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Civil Engineering: Specialist Concrete Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-9",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Civil Engineering: Steelfixing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-10",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Civil Engineering: Steelfixing Occupations Major Projects - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "520-3-6",
                FundingCap = 5000,
                Duration = 23,
                Title = " Construction Civil Engineering: Tunnelling Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "612-20-1",
                FundingCap = 12000,
                Duration = 12,
                Title =
                    " Construction Management: Construction and Building Services Management and Supervision(Sustainability) - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "612-22-1",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Construction Site Management - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "612-20-2",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Construction Site Supervision - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "612-21-3",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Foundation Degree in Architecture - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "612-21-4",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Foundation Degree in Built Environment - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "612-21-5",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Foundation Degree in Civil Engineering - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "612-21-1",
                FundingCap = 12000,
                Duration = 12,
                Title =
                    " Construction Management: Foundation Degree Professional Practice in Construction Operations Management - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "612-22-2",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Management Quantity Surveying and Commercial Management - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "612-21-2",
                FundingCap = 12000,
                Duration = 12,
                Title = " Construction Management: Sustainability - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-1",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Accessing and Rigging - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-2",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Applied Waterproof Membranes - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-3",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Cladding Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-5",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Specialist: Fitted Interiors - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-6",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Specialist: Floorcovering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-5",
                FundingCap = 6000,
                Duration = 30,
                Title = " Construction Specialist: Heritage Skills - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-12",
                FundingCap = 3500,
                Duration = 12,
                Title = " Construction Specialist: Insulation and Building Treatments - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-7",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Specialist: Interior Systems - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-8",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Mastic Asphalting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-6",
                FundingCap = 9000,
                Duration = 30,
                Title = " Construction Specialist: Mastic Asphalting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-9",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Plastering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-1",
                FundingCap = 6000,
                Duration = 30,
                Title = " Construction Specialist: Plastering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-10",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Roofing Occupations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-2",
                FundingCap = 12000,
                Duration = 30,
                Title = " Construction Specialist: Roofing Occupations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-11",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Specialist: Stonemasonry - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-3",
                FundingCap = 9000,
                Duration = 30,
                Title = " Construction Specialist: Stonemasonry - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-13",
                FundingCap = 4000,
                Duration = 18,
                Title = " Construction Specialist: Thermal Insulation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-7",
                FundingCap = 6000,
                Duration = 30,
                Title = " Construction Specialist: Thermal Insulation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "519-3-4",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Specialist: Wall and Floor Tiling - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "519-2-4",
                FundingCap = 9000,
                Duration = 30,
                Title = " Construction Specialist: Wall and Floor Tiling - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "171",
                FundingCap = 12000,
                Duration = 18,
                Title = " Construction Steel Fixer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-6",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Technical and Professional: Building Control - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-1",
                FundingCap = 6000,
                Duration = 18,
                Title = " Construction Technical and Professional: Built Environment and Design - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-4",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Technical and Professional: Civil Engineering for Technicians - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-2",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Technical and Professional: Construction Contracting Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-7",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Technical and Professional: Geomatics Data Analysis - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-5",
                FundingCap = 5000,
                Duration = 18,
                Title = " Construction Technical and Professional: Occupational Work Supervision - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "521-2-8",
                FundingCap = 9000,
                Duration = 18,
                Title = " Construction Technical and Professional: Town Planning Technical Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "587-3-1",
                FundingCap = 2000,
                Duration = 24,
                Title = " Consumer Electrical and Electronic Products: Delivering and Installing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "587-2-1",
                FundingCap = 3500,
                Duration = 24,
                Title = " Consumer Electrical and Electronic Products: Repairing Component Faults - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "587-3-2",
                FundingCap = 3000,
                Duration = 24,
                Title = " Consumer Electrical and Electronic Products: Repairing Module Faults - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "489-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Contact Centre Operations: Contact Centre Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "489-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Contact Centre Operations: Contact Centre Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "9",
                FundingCap = 27000,
                Duration = 60,
                Title = " Control / technical support engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "39",
                FundingCap = 9000,
                Duration = 12,
                Title = " Conveyancing technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "406-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Court, Tribunal and Prosecution Operations: Court and Tribunal Administration - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "406-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Court, Tribunal and Prosecution Operations: Court and Tribunal Administration - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "449-2-1",
                FundingCap = 4000,
                Duration = 15,
                Title = " Creative and Digital Media: Creative and Digital Media - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "449-20-1",
                FundingCap = 2500,
                Duration = 15,
                Title = " Creative and Digital Media: Interactive Design and Development - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "77",
                FundingCap = 5000,
                Duration = 12,
                Title = " Credit controller / collector - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "495-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = " Cultural and Heritage Venue Operations: Cultural and Heritage Venue Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "495-2-2",
                FundingCap = 2500,
                Duration = 12,
                Title = " Cultural and Heritage Venue Operations: Cultural and Heritage Venue Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "495-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Cultural and Heritage Venue Operations: Cultural Heritage - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "495-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Cultural and Heritage Venue Operations: Cultural Heritage - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "410-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Custodial Care: Custodial Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "410-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Custodial Care: Custodial Care - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "122",
                FundingCap = 4000,
                Duration = 12,
                Title = " Customer service practitioner - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "488-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Customer Service: Customer Service - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "488-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Customer Service: Customer Service - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "79",
                FundingCap = 18000,
                Duration = 24,
                Title = " Cyber intrusion analyst - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "98",
                FundingCap = 18000,
                Duration = 24,
                Title = " Cyber security technologist - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "80",
                FundingCap = 15000,
                Duration = 24,
                Title = " Data analyst - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "19",
                FundingCap = 5000,
                Duration = 18,
                Title = " Dental laboratory assistant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "61",
                FundingCap = 9000,
                Duration = 18,
                Title = " Dental nurse - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "20",
                FundingCap = 9000,
                Duration = 24,
                Title = " Dental practice manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "18",
                FundingCap = 18000,
                Duration = 36,
                Title = " Dental technician - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "497-2-1",
                FundingCap = 5000,
                Duration = 12,
                Title = " Design: Design - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "497-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Design: Design Support - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "25",
                FundingCap = 27000,
                Duration = 36,
                Title = " Digital and technology solutions professional - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "636-2-1",
                FundingCap = 4000,
                Duration = 24,
                Title = " Digital Learning Design: Digital Learning Design - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "636-20-1",
                FundingCap = 2000,
                Duration = 24,
                Title = " Digital Learning Design: Digital Learning Design - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "78",
                FundingCap = 12000,
                Duration = 18,
                Title = " Digital marketer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "516-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = " Domestic Heating: Domestic Heating - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "516-2-2",
                FundingCap = 6000,
                Duration = 24,
                Title = " Domestic Heating: Gas - Fired Warm Air Appliances - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "516-2-3",
                FundingCap = 6000,
                Duration = 24,
                Title = " Domestic Heating: Gas - Fired Water and Central Heating Appliances - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "516-2-1",
                FundingCap = 6000,
                Duration = 24,
                Title = " Domestic Heating: Oil - Fired, Solid Fuel or Environmental Options - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-3-4",
                FundingCap = 2000,
                Duration = 12,
                Title = " Driving Goods Vehicles: Articulated / Drawbar Driver - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-3",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Articulated / Drawbar Driver - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Driving Goods Vehicles: Motorcycle / Cycle Courier - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "441-3-3",
                FundingCap = 2500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Rigid Vehicle Driver - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Rigid Vehicle Driver - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-6",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Transporting Freight by Road(Articulated / Drawbar) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-5",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Transporting Freight by Road(Rigid) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-4",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Transporting Freight by Road(Van) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "441-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = " Driving Goods Vehicles: Van Driver - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "441-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Driving Goods Vehicles: Van Driver - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "26",
                FundingCap = 12000,
                Duration = 14,
                Title = " Dual fuel smart meter installer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "10",
                FundingCap = 27000,
                Duration = 60,
                Title = " Electrical / electronic technical support engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "216",
                FundingCap = 27000,
                Duration = 30,
                Title = " Electrical Power Networks Engineer - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "127",
                FundingCap = 27000,
                Duration = 36,
                Title = " Electrical power protection and plant commissioning engineer - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "212",
                FundingCap = 9000,
                Duration = 36,
                Title = " Electrical, Electronic Product Service and Installation Engineer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "513-2-1",
                FundingCap = 12000,
                Duration = 42,
                Title = " Electrotechnical: Electrical Installation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "107",
                FundingCap = 27000,
                Duration = 36,
                Title = " Embedded electronic systems design and development engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "409-2-1",
                FundingCap = 3000,
                Duration = 24,
                Title = " Emergency Fire Service Operations: Emergency Fire Services Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "536-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Employment Related Services: Employment Related Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "202",
                FundingCap = 21000,
                Duration = 42,
                Title = " Engineering Construction Pipefitter - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-16",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Construction and Completions Control - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-13",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Design and Draughting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-8",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Electrical Installation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-11",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Construction: Electrical Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-10",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Construction: Instrument and Control - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-15",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Instrument Pipefitting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-2",
                FundingCap = 9000,
                Duration = 36,
                Title = " Engineering Construction: Mechanical Fitting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-12",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Construction: Mechanical Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-9",
                FundingCap = 6000,
                Duration = 36,
                Title = " Engineering Construction: Non Destructive Testing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-1",
                FundingCap = 9000,
                Duration = 36,
                Title = " Engineering Construction: Pipefitting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-5",
                FundingCap = 9000,
                Duration = 36,
                Title = " Engineering Construction: Plating - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-14",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Project Control - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-7",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Rigging(Moving Loads) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-6",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Steel Erecting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-3",
                FundingCap = 12000,
                Duration = 36,
                Title = " Engineering Construction: Welding(Pipework) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "517-2-4",
                FundingCap = 9000,
                Duration = 36,
                Title = " Engineering Construction: Welding(Plate) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "92",
                FundingCap = 27000,
                Duration = 42,
                Title = " Engineering design and draughtsperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-1",
                FundingCap = 5000,
                Duration = 18,
                Title = " Engineering Manufacture: Aerospace - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-1",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Aerospace - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-12",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Automotive - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-9",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Electrical and Electronic Engineering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-14",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Engineering Leadership - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-5",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Engineering Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-4",
                FundingCap = 4000,
                Duration = 18,
                Title = " Engineering Manufacture: Engineering Maintenance and Installation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-7",
                FundingCap = 5000,
                Duration = 18,
                Title = " Engineering Manufacture: Engineering Technical Support - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-8",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Engineering Technical Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-11",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Engineering Toolmaking - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-13",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Engineering Woodworking, Pattern and Modelmaking - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-5",
                FundingCap = 5000,
                Duration = 18,
                Title = " Engineering Manufacture: Fabrication and Welding - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-6",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Fabrication and Welding - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-10",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Installation and Commissioning - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-2",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Marine(Ship Building, Maintenance and Repair) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-2",
                FundingCap = 5000,
                Duration = 18,
                Title = " Engineering Manufacture: Marine(Ship, Yacht, Boatbuilding, Maintenance and Repair) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-4",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Marine(Yacht and Boat Building, Maintenance and Repair) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-6",
                FundingCap = 4000,
                Duration = 18,
                Title = " Engineering Manufacture: Materials Processing and Finishing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-7",
                FundingCap = 9000,
                Duration = 42,
                Title = " Engineering Manufacture: Materials Processing and Finishing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "539-3-3",
                FundingCap = 4000,
                Duration = 18,
                Title = " Engineering Manufacture: Mechanical Manufacturing Engineering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "539-2-3",
                FundingCap = 12000,
                Duration = 42,
                Title = " Engineering Manufacture: Mechanical Manufacturing Engineering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "167",
                FundingCap = 27000,
                Duration = 42,
                Title = " Engineering Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "524-3-2",
                FundingCap = 1500,
                Duration = 15,
                Title = " Environmental Conservation: Dry Stone Walling - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "524-2-2",
                FundingCap = 1500,
                Duration = 18,
                Title = " Environmental Conservation: Dry Stone Walling - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "524-3-1",
                FundingCap = 1500,
                Duration = 15,
                Title = " Environmental Conservation: Environmental Conservation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "524-2-1",
                FundingCap = 1500,
                Duration = 18,
                Title = " Environmental Conservation: Environmental Conservation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "511-3-3",
                FundingCap = 2000,
                Duration = 18,
                Title = " Equine: Harness Horse Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "511-2-3",
                FundingCap = 2500,
                Duration = 24,
                Title = " Equine: Harness Horse Care and Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "511-3-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Equine: Horse Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "511-2-1",
                FundingCap = 3000,
                Duration = 24,
                Title = " Equine: Horse Care and Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "511-3-2",
                FundingCap = 2000,
                Duration = 18,
                Title = " Equine: Racehorse Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "511-2-2",
                FundingCap = 3000,
                Duration = 24,
                Title = " Equine: Racehorse Care and Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "159",
                FundingCap = 9000,
                Duration = 18,
                Title = " Event Assistant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "462-3-1",
                FundingCap = 2500,
                Duration = 12,
                Title = " Exercise and Fitness: Exercise and Fitness - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "462-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Exercise and Fitness: Personal Training - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "162",
                FundingCap = 4000,
                Duration = 18,
                Title = " Facilities Management Supervisor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "501-21-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Facilities Management: Building Services - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "501-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Facilities Management: Facilities Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "501-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Facilities Management: Facilities Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "501-20-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Facilities Management: Generic - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "501-21-1",
                FundingCap = 6000,
                Duration = 12,
                Title = " Facilities Management: Generic - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "526-2-1",
                FundingCap = 9000,
                Duration = 50,
                Title = " Farriery: Farriery - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = " Fashion and Textiles: Apparel - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-2",
                FundingCap = 12000,
                Duration = 24,
                Title = " Fashion and Textiles: Apparel - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-4",
                FundingCap = 2500,
                Duration = 12,
                Title = " Fashion and Textiles: Footwear - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-4",
                FundingCap = 9000,
                Duration = 24,
                Title = " Fashion and Textiles: Footwear - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-3",
                FundingCap = 2500,
                Duration = 12,
                Title = " Fashion and Textiles: Leather goods - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-3",
                FundingCap = 12000,
                Duration = 24,
                Title = " Fashion and Textiles: Leather goods - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-9",
                FundingCap = 2000,
                Duration = 12,
                Title = " Fashion and Textiles: Leather Production - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-7",
                FundingCap = 12000,
                Duration = 24,
                Title = " Fashion and Textiles: Leather Production - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-5",
                FundingCap = 4000,
                Duration = 12,
                Title = " Fashion and Textiles: Saddlery - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-6",
                FundingCap = 9000,
                Duration = 24,
                Title = " Fashion and Textiles: Tailoring - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "423-3-1",
                FundingCap = 2500,
                Duration = 12,
                Title = " Fashion and Textiles: Textiles - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "423-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = " Fashion and Textiles: Textiles - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "530-3-1",
                FundingCap = 3500,
                Duration = 15,
                Title = " Fencing: Fencing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "530-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Fencing: Fencing Supervision - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "153",
                FundingCap = 9000,
                Duration = 24,
                Title = " Financial Adviser - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "8",
                FundingCap = 12000,
                Duration = 12,
                Title = " Financial services administrator - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "28",
                FundingCap = 4000,
                Duration = 12,
                Title = " Financial services customer adviser - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "213",
                FundingCap = 18000,
                Duration = 42,
                Title = " Financial Services Professional - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "126",
                FundingCap = 18000,
                Duration = 36,
                Title = " Fire emergency and security systems technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "172",
                FundingCap = 12000,
                Duration = 24,
                Title = " Fishmonger - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "523-3-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Floristry: Floristry - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "523-2-1",
                FundingCap = 2500,
                Duration = 24,
                Title = " Floristry: Floristry - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "129",
                FundingCap = 9000,
                Duration = 24,
                Title = " Food and drink advanced process operator - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "16",
                FundingCap = 27000,
                Duration = 36,
                Title = " Food and drink maintenance engineer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "130",
                FundingCap = 5000,
                Duration = 30,
                Title = " Food and drink process operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Baking Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-2",
                FundingCap = 9000,
                Duration = 12,
                Title = " Food and Drink: Baking Industry Skills - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-7",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Brewing Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-8",
                FundingCap = 2000,
                Duration = 12,
                Title = " Food and Drink: Dairy Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-6",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Fish and Shellfish Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-7",
                FundingCap = 6000,
                Duration = 12,
                Title = " Food and Drink: Fish and Shellfish Industry Skills - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-4",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Food Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-3",
                FundingCap = 6000,
                Duration = 12,
                Title = " Food and Drink: Food Industry Skills and Technical Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-11",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Food Industry Team Leading - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-5",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Food Manufacturing Excellence - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-4",
                FundingCap = 6000,
                Duration = 12,
                Title = " Food and Drink: Food Manufacturing Excellence - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-9",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Fresh Produce Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-5",
                FundingCap = 6000,
                Duration = 12,
                Title = " Food and Drink: Fresh Produce Industry Skills - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-10",
                FundingCap = 2000,
                Duration = 12,
                Title = " Food and Drink: Livestock Market Droving Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Food and Drink: Meat and Poultry Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "403-2-1",
                FundingCap = 9000,
                Duration = 12,
                Title = " Food and Drink: Meat and Poultry Industry Skills - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "403-3-3",
                FundingCap = 2000,
                Duration = 12,
                Title = " Food and Drink: Milling Industry Skills - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "184",
                FundingCap = 27000,
                Duration = 48,
                Title = " Food Industry Technical Professional - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "131",
                FundingCap = 18000,
                Duration = 36,
                Title = " Food technologist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "182",
                FundingCap = 12000,
                Duration = 24,
                Title = " Forest Operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "578-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Funeral Operations and Services: Funeral Operations and Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "578-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Funeral Operations and Services: Funeral Operations and Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "137",
                FundingCap = 9000,
                Duration = 24,
                Title = " Furniture manufacturer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-2",
                FundingCap = 5000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Finishing Furniture - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-2",
                FundingCap = 6000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Finishing Furniture - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-4",
                FundingCap = 4000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Fitted Furniture and Interiors - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-4",
                FundingCap = 6000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Fitted Furniture and Interiors - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-8",
                FundingCap = 6000,
                Duration = 18,
                Title =
                    " Furniture, Furnishings and Interiors Manufacturing: Furniture and Wood Processing - CNC Machining - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-7",
                FundingCap = 6000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Furniture and Wood Processing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-5",
                FundingCap = 6000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Furniture Design - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-1",
                FundingCap = 5000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Furniture Making - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Furniture Making - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-3",
                FundingCap = 4000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Restoring Furniture - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-3",
                FundingCap = 5000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Restoring Furniture - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-9",
                FundingCap = 2000,
                Duration = 18,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Solid Surfaces - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-7",
                FundingCap = 6000,
                Duration = 24,
                Title =
                    " Furniture, Furnishings and Interiors Manufacturing: Supervision in the Furniture, Furnishings and Interiors Industry - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-5",
                FundingCap = 5000,
                Duration = 18,
                Title =
                    " Furniture, Furnishings and Interiors Manufacturing: Upholstery and Soft Furnishings - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-6",
                FundingCap = 6000,
                Duration = 24,
                Title =
                    " Furniture, Furnishings and Interiors Manufacturing: Upholstery and Soft Furnishings - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "551-3-6",
                FundingCap = 5000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Wood Machining - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "551-2-8",
                FundingCap = 6000,
                Duration = 24,
                Title = " Furniture, Furnishings and Interiors Manufacturing: Wood Machining - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "438-3-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Game and Wildlife Management: Game and Wildlife Management - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "438-2-1",
                FundingCap = 2000,
                Duration = 24,
                Title = " Game and Wildlife Management: Game and Wildlife Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "74",
                FundingCap = 27000,
                Duration = 18,
                Title = " Gas engineering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "57",
                FundingCap = 27000,
                Duration = 48,
                Title = " Gas network craftsperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "58",
                FundingCap = 9000,
                Duration = 12,
                Title = " Gas network team leader - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-1",
                FundingCap = 3000,
                Duration = 24,
                Title = " Glass Industry: Automotive Glazing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Curtain Wall Installation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-2",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Domestic Fascia, Soffit and Bargeboard Installation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-6",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Fabrication of Glass Supporting Structures - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-2",
                FundingCap = 9000,
                Duration = 24,
                Title = " Glass Industry: Fabrication of Glass Supporting Structures - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-5",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Fenestration Installation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-4",
                FundingCap = 9000,
                Duration = 24,
                Title = " Glass Industry: Fenestration Installation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-5",
                FundingCap = 4000,
                Duration = 24,
                Title = " Glass Industry: Fenestration Surveyor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-7",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Flat Glass Manufacture - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-8",
                FundingCap = 3500,
                Duration = 24,
                Title = " Glass Industry: Glass Manufacturing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-3",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Glass Processing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-6",
                FundingCap = 4000,
                Duration = 24,
                Title = " Glass Industry: Glass Processor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-13",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Glass Related Distribution and Warehousing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-7",
                FundingCap = 3500,
                Duration = 24,
                Title = " Glass Industry: Glass Related Distribution and Warehousing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-9",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Glazing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "502-2-3",
                FundingCap = 9000,
                Duration = 24,
                Title = " Glass Industry: Glazing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "502-3-11",
                FundingCap = 3000,
                Duration = 12,
                Title = " Glass Industry: Photovoltaics Installation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "21",
                FundingCap = 6000,
                Duration = 24,
                Title = " Golf greenkeeper - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "157",
                FundingCap = 9000,
                Duration = 24,
                Title = " Hair Professional - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "508-3-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Hairdressing: Hairdressing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "508-2-1",
                FundingCap = 3000,
                Duration = 12,
                Title = " Hairdressing: Hairdressing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "478-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Health Allied Health Profession Support: Health Allied Health Profession Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "444-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Health and Social Care: Adult social care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "444-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Health and Social Care: Adult social care - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "444-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = " Health and Social Care: Health - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "444-2-2",
                FundingCap = 1500,
                Duration = 18,
                Title = " Health and Social Care: Health - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "602-21-1",
                FundingCap = 5000,
                Duration = 12,
                Title = " Health Assistant Practitioner: Health Assistant Practitioner - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "473-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Health Clinical Healthcare Support: Health Clinical Healthcare Support - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "473-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Health Clinical Healthcare Support: Health Clinical Healthcare Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "479-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Health Dental Nursing: Health Dental Nursing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "476-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = " Health Emergency Care: Health Emergency Care - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "474-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Health Healthcare Support Services: Health Healthcare Support Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "474-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Health Healthcare Support Services: Health Healthcare Support Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "475-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = " Health Maternity and Paediatric Support: Health Maternity and Paediatric Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "471-3-1",
                FundingCap = 2000,
                Duration = 15,
                Title = " Health Optical Retail: Health Optical Retail - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "471-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Health Optical Retail: Health Optical Retail - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "477-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = " Health Perioperative Support: Health Perioperative Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "480-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = " Health Pharmacy Services: Health Pharmacy Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "480-2-1",
                FundingCap = 4000,
                Duration = 18,
                Title = " Health Pharmacy Services: Health Pharmacy Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "102",
                FundingCap = 12000,
                Duration = 18,
                Title = " Healthcare assistant practitioner - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "99",
                FundingCap = 5000,
                Duration = 12,
                Title = " Healthcare science assistant - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "150",
                FundingCap = 9000,
                Duration = 24,
                Title = " Healthcare science associate - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "168",
                FundingCap = 27000,
                Duration = 36,
                Title = " Healthcare Science Practitioner - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "103",
                FundingCap = 3000,
                Duration = 12,
                Title = " Healthcare support worker - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "515-3-1",
                FundingCap = 6000,
                Duration = 24,
                Title = " Heating and Ventilating: Ductwork Systems - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "515-2-1",
                FundingCap = 6000,
                Duration = 24,
                Title = " Heating and Ventilating: Ductwork Systems - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "515-3-2",
                FundingCap = 5000,
                Duration = 24,
                Title = " Heating and Ventilating: Pipework Systems - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "515-2-2",
                FundingCap = 6000,
                Duration = 24,
                Title = " Heating and Ventilating: Pipework Systems - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "515-3-3",
                FundingCap = 5000,
                Duration = 24,
                Title = " Heating and Ventilating: Servicing and Maintaining H and V Systems - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "515-2-3",
                FundingCap = 6000,
                Duration = 24,
                Title = " Heating and Ventilating: Servicing and Maintaining H and V Systems - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "135",
                FundingCap = 18000,
                Duration = 36,
                Title = " Heavy vehicle service and maintenance technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "220",
                FundingCap = 21000,
                Duration = 36,
                Title = " High Speed Rail and Infrastructure Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "125",
                FundingCap = 9000,
                Duration = 24,
                Title = " Highway electrical maintenance and installation operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "124",
                FundingCap = 9000,
                Duration = 24,
                Title = " Highways electrician / service operative - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "108",
                FundingCap = 2500,
                Duration = 12,
                Title = "HM Forces serviceperson(public services) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "537-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "HM Forces: HM Forces - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "181",
                FundingCap = 5000,
                Duration = 24,
                Title = "Horticulture and Landscape Operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "527-3-1",
                FundingCap = 2000,
                Duration = 20,
                Title = "Horticulture: Horticulture - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "527-2-1",
                FundingCap = 2500,
                Duration = 24,
                Title = "Horticulture: Horticulture - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "580-20-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Hospitality Management: Hospitality Management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "223",
                FundingCap = 6000,
                Duration = 18,
                Title = "Hospitality Manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "138",
                FundingCap = 5000,
                Duration = 12,
                Title = "Hospitality supervisor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "96",
                FundingCap = 5000,
                Duration = 12,
                Title = "Hospitality team member - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "583-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = "Hospitality: Food and Beverage Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "583-3-4",
                FundingCap = 2000,
                Duration = 12,
                Title = "Hospitality: Front of House Reception - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "583-2-2",
                FundingCap = 2500,
                Duration = 12,
                Title = "Hospitality: Hospitality Retail Outlet Supervision - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "583-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Hospitality: Hospitality Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "583-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Hospitality: Hospitality Supervision and Leadership - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "583-3-3",
                FundingCap = 1500,
                Duration = 12,
                Title = "Hospitality: House Keeping - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "65",
                FundingCap = 9000,
                Duration = 18,
                Title = "Housing / property management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "64",
                FundingCap = 3000,
                Duration = 12,
                Title = "Housing / property management assistant - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "499-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Housing: Housing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "190",
                FundingCap = 9000,
                Duration = 36,
                Title = "HR Consultant / Partner - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "191",
                FundingCap = 5000,
                Duration = 18,
                Title = "HR Support - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "574-21-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Human Resource Management: Human Resource Management - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "504-3-1",
                FundingCap = 4000,
                Duration = 12,
                Title = "Improving Operational Performance: Performing Engineering Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "504-3-2",
                FundingCap = 3500,
                Duration = 12,
                Title = "Improving Operational Performance: Performing Manufacturing Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "82",
                FundingCap = 15000,
                Duration = 12,
                Title = "Infrastructure technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "5",
                FundingCap = 18000,
                Duration = 42,
                Title = "Installation electrician / maintenance electrician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "60",
                FundingCap = 9000,
                Duration = 12,
                Title = "Insurance practitioner - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "63",
                FundingCap = 9000,
                Duration = 24,
                Title = "Insurance professional - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "637-20-1",
                FundingCap = 3000,
                Duration = 12,
                Title = "Intelligence Operations: Intelligence Operations - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "413-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title =
                    "International Trade and Logistics Operations: International Trade and Logistics Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "29",
                FundingCap = 5000,
                Duration = 12,
                Title = "Investment operations administrator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "30",
                FundingCap = 9000,
                Duration = 18,
                Title = "Investment operations specialist - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "33",
                FundingCap = 9000,
                Duration = 18,
                Title = "Investment operations technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "419-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "IT Application Specialist: IT Application Specialist - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "419-2-1",
                FundingCap = 4000,
                Duration = 12,
                Title = "IT Application Specialist: IT Application Specialist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "142",
                FundingCap = 12000,
                Duration = 12,
                Title = "IT technical salesperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "418-3-1",
                FundingCap = 4000,
                Duration = 12,
                Title =
                    "IT, Software, Web and Telecoms Professionals: IT, Software, Web and Telecoms Professionals - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "418-2-1",
                FundingCap = 9000,
                Duration = 12,
                Title =
                    "IT, Software, Web and Telecoms Professionals: IT, Software, Web and Telecoms Professionals - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "418-20-1",
                FundingCap = 12000,
                Duration = 18,
                Title =
                    "IT, Software, Web and Telecoms Professionals: IT, Software, Web and Telecoms Professionals - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "548-3-1",
                FundingCap = 3500,
                Duration = 15,
                Title = "Jewellery, Silversmithing and Allied Trades: Jewellery Manufacturing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "548-2-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Jewellery, Silversmithing and Allied Trades: Jewellery Manufacturing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "548-2-3",
                FundingCap = 5000,
                Duration = 24,
                Title = "Jewellery, Silversmithing and Allied Trades: Precious Metal CAD/CAM - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "548-2-2",
                FundingCap = 6000,
                Duration = 24,
                Title = "Jewellery, Silversmithing and Allied Trades: Silversmithing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "84",
                FundingCap = 9000,
                Duration = 18,
                Title = "Junior 2D artist(visual effects) - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "174",
                FundingCap = 12000,
                Duration = 12,
                Title = "Junior Content Producer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "68",
                FundingCap = 9000,
                Duration = 24,
                Title = "Junior energy manager - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "22",
                FundingCap = 12000,
                Duration = 18,
                Title = "Junior journalist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "83",
                FundingCap = 9000,
                Duration = 24,
                Title = "Junior management consultant - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "506-3-1",
                FundingCap = 4000,
                Duration = 18,
                Title = "Laboratory and Science Technicians: Education Science - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "506-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = "Laboratory and Science Technicians: Education Science - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "506-3-2",
                FundingCap = 4000,
                Duration = 18,
                Title = "Laboratory and Science Technicians: Industrial Science - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "506-2-2",
                FundingCap = 12000,
                Duration = 24,
                Title = "Laboratory and Science Technicians: Industrial Science - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "44",
                FundingCap = 27000,
                Duration = 60,
                Title = "Laboratory scientist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "221",
                FundingCap = 27000,
                Duration = 60,
                Title = "Laboratory Scientist - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "14",
                FundingCap = 21000,
                Duration = 18,
                Title = "Laboratory technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "525-3-1",
                FundingCap = 4000,
                Duration = 24,
                Title = "Land-based Engineering: Land-based Engineering - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "525-2-1",
                FundingCap = 4000,
                Duration = 15,
                Title = "Land-based Engineering: Land-based Engineering - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "69",
                FundingCap = 18000,
                Duration = 18,
                Title = "Land-based service engineer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "70",
                FundingCap = 27000,
                Duration = 36,
                Title = "Land-based service engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "110",
                FundingCap = 5000,
                Duration = 12,
                Title = "Large goods vehicle(LGV) driver - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "118",
                FundingCap = 3000,
                Duration = 12,
                Title = "Lead adult care worker - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "541-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Learning and Development: Learning and Development - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-2",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Civil Litigation - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-20-1",
                FundingCap = 2000,
                Duration = 24,
                Title = "Legal Services: Commercial Litigation - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Legal Services: Criminal Prosecution - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-20-2",
                FundingCap = 3500,
                Duration = 24,
                Title = "Legal Services: Debt Recovery and Insolvency - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-3",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Employment Practice - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-4",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Family Practice - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-7",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Paralegal Practice - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-20-3",
                FundingCap = 4000,
                Duration = 24,
                Title = "Legal Services: Personal Injury - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-6",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Private Client Practice - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "565-2-5",
                FundingCap = 3000,
                Duration = 18,
                Title = "Legal Services: Property - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "224",
                FundingCap = 21000,
                Duration = 36,
                Title = "Leisure and Entertainment Maintenance Engineering Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "463-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Leisure Management: Leisure Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "466-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Leisure Operations and Leisure Management: Leisure Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "466-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Leisure Operations and Leisure Management: Leisure Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "450-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title =
                    "Libraries, Archives, Records and Information Management Services: Libraries, Archives, Records and Information Management Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "450-2-1",
                FundingCap = 3000,
                Duration = 12,
                Title =
                    "Libraries, Archives, Records and Information Management Services: Libraries, Archives, Records and Information Management Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "40",
                FundingCap = 9000,
                Duration = 18,
                Title = "Licensed conveyancer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "404-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Licensed Hospitality: Licensed Hospitality - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "404-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Licensed Hospitality: Licensed Hospitality Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "563-20-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Chemical Science Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "563-21-1",
                FundingCap = 15000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Chemical Science Technologist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "563-21-6",
                FundingCap = 12000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Food Science Technologist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "563-20-3",
                FundingCap = 5000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Healthcare Science Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "563-21-3",
                FundingCap = 12000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Healthcare Science Technologist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "563-20-2",
                FundingCap = 5000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Life Science Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "563-21-2",
                FundingCap = 15000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Life Science Technologist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "563-20-4",
                FundingCap = 9000,
                Duration = 24,
                Title = "Life Science and Chemical Science Professionals: Process Development Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "219",
                FundingCap = 15000,
                Duration = 18,
                Title = "Lifting Technician - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "71",
                FundingCap = 9000,
                Duration = 36,
                Title = "Live event rigger - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "491-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Live Events and Promotion: Live Events and Promotion - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "491-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Live Events and Promotion: Live Events and Promotion - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "442-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Logistics Operations: Logistics Operations Team Leader/Section Supervisor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "442-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Logistics Operations: Logistics Operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "442-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = "Logistics Operations: Logistics Support Operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "146",
                FundingCap = 27000,
                Duration = 36,
                Title = "Maintenance and operations engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "487-21-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Management: Leadership and Management - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "487-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = "Management: Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "487-20-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Management: Management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "487-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Management: Team Leading - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "487-3-2",
                FundingCap = 3500,
                Duration = 18,
                Title = "Management: Team Leading (Construction) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "11",
                FundingCap = 27000,
                Duration = 60,
                Title = "Manufacturing engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-1",
                FundingCap = 9000,
                Duration = 12,
                Title = "Manufacturing Engineering: Aerospace - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-7",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Automotive - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-5",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Electrical/Electronics - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-6",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Electrical/Electronics continuation of Pathway 5 - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-8",
                FundingCap = 9000,
                Duration = 12,
                Title = "Manufacturing Engineering: Maintenance - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-11",
                FundingCap = 9000,
                Duration = 12,
                Title = "Manufacturing Engineering: Marine - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-3",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Mechanical - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-4",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Mechanical continuation of Pathway 3 - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-2",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Nuclear Related Technology - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-13",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Rail Engineering - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-12",
                FundingCap = 9000,
                Duration = 12,
                Title = "Manufacturing Engineering: Space Engineering - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "550-20-9",
                FundingCap = 12000,
                Duration = 12,
                Title = "Manufacturing Engineering: Wind Generation - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "226",
                FundingCap = 27000,
                Duration = 48,
                Title = "Marine Engineer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-4",
                FundingCap = 4000,
                Duration = 12,
                Title = "Maritime Occupations: Able Seafarer/tug rating - engine room - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-7",
                FundingCap = 4000,
                Duration = 12,
                Title = "Maritime Occupations: Marinas and Boatyards - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "560-2-1",
                FundingCap = 5000,
                Duration = 12,
                Title = "Maritime Occupations: Merchant Navy(Deck) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "560-2-2",
                FundingCap = 5000,
                Duration = 12,
                Title = "Maritime Occupations: Merchant Navy(Engineering) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "560-2-4",
                FundingCap = 5000,
                Duration = 18,
                Title =
                    "Maritime Occupations: Officer of the watch on merchant vessels of less than 3,000 gross tonnage - near coastal - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "560-2-3",
                FundingCap = 5000,
                Duration = 12,
                Title =
                    "Maritime Occupations: Officer of the watch on merchant vessels of less than 500 gross tonnage - near coastal - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-5",
                FundingCap = 3500,
                Duration = 15,
                Title = "Maritime Occupations: Port Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-1",
                FundingCap = 5000,
                Duration = 12,
                Title = "Maritime Occupations: Rivers, inland waterways and limited distances to sea - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-2",
                FundingCap = 5000,
                Duration = 12,
                Title = "Maritime Occupations: Sea Fishing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "560-3-6",
                FundingCap = 5000,
                Duration = 12,
                Title = "Maritime Occupations: Workboat Operation - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "486-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Marketing: Marketing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "486-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = "Marketing: Marketing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "187",
                FundingCap = 24000,
                Duration = 36,
                Title = "Metrology Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "175",
                FundingCap = 9000,
                Duration = 18,
                Title = "Mineral Processing Mobile and Static Plant Operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "593-21-1",
                FundingCap = 12000,
                Duration = 24,
                Title = "Mineral Products Technology: Technical and Management - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "593-20-1",
                FundingCap = 15000,
                Duration = 24,
                Title =
                    "Mineral Products Technology: Technical and Managerial Development in Mineral Products Technology Industry - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "62",
                FundingCap = 9000,
                Duration = 12,
                Title = "Mortgage adviser - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "210",
                FundingCap = 6000,
                Duration = 24,
                Title = "Motor Finance Specialist - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "59",
                FundingCap = 18000,
                Duration = 36,
                Title = "Motor vehicle service and maintenance technician(light vehicle) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "509-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Nail Services: Nail Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "509-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Nail Services: Nail Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "1",
                FundingCap = 18000,
                Duration = 24,
                Title = "Network engineer - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "132",
                FundingCap = 12000,
                Duration = 18,
                Title = "Non-destructive testing(NDT) operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "207",
                FundingCap = 27000,
                Duration = 48,
                Title = "Non-Destructive Testing Engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "67",
                FundingCap = 18000,
                Duration = 36,
                Title = "Non-destructive testing engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "46",
                FundingCap = 9000,
                Duration = 24,
                Title = "Nuclear health physics monitor - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "47",
                FundingCap = 27000,
                Duration = 36,
                Title = "Nuclear scientist and nuclear engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "163",
                FundingCap = 21000,
                Duration = 30,
                Title = "Nuclear Technician - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "35",
                FundingCap = 27000,
                Duration = 48,
                Title = "Nuclear welding inspection technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "569-3-1",
                FundingCap = 2000,
                Duration = 15,
                Title =
                    "Nursing Assistants in a Veterinary Environment: Nursing Assistants in a Veterinary Environment - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "208",
                FundingCap = 15000,
                Duration = 24,
                Title = "Nursing Associate - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "104",
                FundingCap = 9000,
                Duration = 30,
                Title = "Operations / departmental manager - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "197",
                FundingCap = 24000,
                Duration = 36,
                Title = "Organ Builder - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "464-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = "Outdoor Programmes: Outdoor Programmes - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "75",
                FundingCap = 9000,
                Duration = 12,
                Title = "Outside broadcasting engineer - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "106",
                FundingCap = 6000,
                Duration = 36,
                Title = "Papermaker - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "42",
                FundingCap = 9000,
                Duration = 24,
                Title = "Paralegal - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "48",
                FundingCap = 9000,
                Duration = 24,
                Title = "Paraplanner - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "430-3-1",
                FundingCap = 3500,
                Duration = 12,
                Title = "Passenger Carrying Vehicle Driving(Bus and Coach): Bus and Coach - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "189",
                FundingCap = 6000,
                Duration = 12,
                Title = "Passenger Transport Driver - bus, coach and rail - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "206",
                FundingCap = 6000,
                Duration = 12,
                Title = "Passenger transport onboard and station team member - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "214",
                FundingCap = 12000,
                Duration = 18,
                Title = "Passenger Transport Operations Manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "448-2-1",
                FundingCap = 3500,
                Duration = 12,
                Title = "Photo Imaging: Photo Imaging - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "456-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Playwork: Playwork - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "456-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = "Playwork: Playwork - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "225",
                FundingCap = 21000,
                Duration = 48,
                Title = "Plumbing and Domestic Heating Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "512-2-2",
                FundingCap = 9000,
                Duration = 24,
                Title = "Plumbing and Heating: Domestic Plumbing and Heating Gas-Fired Warm Air Appliance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "512-2-3",
                FundingCap = 9000,
                Duration = 24,
                Title =
                    "Plumbing and Heating: Domestic Plumbing and Heating Gas-Fired Water and Central Heating Appliances - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "512-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title =
                    "Plumbing and Heating: Domestic Plumbing and Heating Oil-Fired, Solid Fuel or Environmental options - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "512-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Plumbing and Heating: Plumbing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "407-2-1",
                FundingCap = 2500,
                Duration = 14,
                Title = "Policing: Defence Policing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "166",
                FundingCap = 27000,
                Duration = 30,
                Title = "Postgraduate Engineer - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "161",
                FundingCap = 27000,
                Duration = 60,
                Title = "Power Engineer - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "6",
                FundingCap = 27000,
                Duration = 30,
                Title = "Power network craftsperson - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-2-5",
                FundingCap = 9000,
                Duration = 24,
                Title = "Print and Printed Packaging: Carton Manufacture - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = "Print and Printed Packaging: Digital Pre-Press - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-3-2",
                FundingCap = 3500,
                Duration = 12,
                Title = "Print and Printed Packaging: Machine Printing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "542-2-2",
                FundingCap = 9000,
                Duration = 24,
                Title = "Print and Printed Packaging: Machine Printing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-3-1",
                FundingCap = 4000,
                Duration = 12,
                Title = "Print and Printed Packaging: Pre-Press - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "542-3-4",
                FundingCap = 3500,
                Duration = 12,
                Title = "Print and Printed Packaging: Print Administration - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "542-2-4",
                FundingCap = 9000,
                Duration = 24,
                Title = "Print and Printed Packaging: Print Administration and Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-3-3",
                FundingCap = 4000,
                Duration = 12,
                Title = "Print and Printed Packaging: Print Finishing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "542-2-3",
                FundingCap = 9000,
                Duration = 24,
                Title = "Print and Printed Packaging: Print Finishing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "542-3-5",
                FundingCap = 4000,
                Duration = 12,
                Title = "Print and Printed Packaging: Reprographics - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "211",
                FundingCap = 5000,
                Duration = 18,
                Title = "Probate Technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "201",
                FundingCap = 27000,
                Duration = 60,
                Title = "Process Automation Engineer - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "425-2-3",
                FundingCap = 12000,
                Duration = 36,
                Title = "Process Manufacturing: Downstream Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "425-3-2",
                FundingCap = 6000,
                Duration = 15,
                Title = "Process Manufacturing: Process Engineering Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "425-2-2",
                FundingCap = 9000,
                Duration = 24,
                Title = "Process Manufacturing: Process Engineering Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "425-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Process Manufacturing: Process Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "425-2-1",
                FundingCap = 12000,
                Duration = 24,
                Title = "Process Manufacturing: Process Operator/Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "12",
                FundingCap = 27000,
                Duration = 60,
                Title = "Product design and development engineer - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "117",
                FundingCap = 9000,
                Duration = 18,
                Title = "Professional accounting taxation technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "575-20-1",
                FundingCap = 3000,
                Duration = 18,
                Title = "Professional Services: Audit - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "575-20-4",
                FundingCap = 3000,
                Duration = 18,
                Title = "Professional Services: Management Accounting - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "575-20-3",
                FundingCap = 3000,
                Duration = 18,
                Title = "Professional Services: Management Consulting - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "575-20-2",
                FundingCap = 3000,
                Duration = 18,
                Title = "Professional Services: Tax - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "186",
                FundingCap = 21000,
                Duration = 42,
                Title = "Project Controls Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "573-20-1",
                FundingCap = 3000,
                Duration = 12,
                Title = "Project Management: Project Management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "23",
                FundingCap = 9000,
                Duration = 12,
                Title = "Property maintenance operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "500-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = "Property Services: Residential Letting and Management - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "500-2-2",
                FundingCap = 2500,
                Duration = 12,
                Title = "Property Services: Residential Letting and Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "500-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Property Services: Sale of Residential Property - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "500-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Property Services: Sale of Residential Property - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-5",
                FundingCap = 2000,
                Duration = 12,
                Title =
                    "Providing Financial Services: Administration for Mortgage and/or Financial Planning Intermediaries - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-5",
                FundingCap = 3000,
                Duration = 18,
                Title =
                    "Providing Financial Services: Administration for Mortgage and/or Financial Planning Intermediaries - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Banking - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-2",
                FundingCap = 3000,
                Duration = 18,
                Title = "Providing Financial Services: Banking - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-7",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Customer Payments for Financial Products and Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-9",
                FundingCap = 3500,
                Duration = 18,
                Title = "Providing Financial Services: Customer Payments for Financial Products and Services - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-8",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Debt Collections - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-8",
                FundingCap = 3000,
                Duration = 18,
                Title = "Providing Financial Services: Debt Collections - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-4",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Financing and Credit - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-4",
                FundingCap = 3500,
                Duration = 18,
                Title = "Providing Financial Services: Financing and Credit - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Providing Financial Services: General Insurance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-1",
                FundingCap = 3000,
                Duration = 18,
                Title = "Providing Financial Services: General Insurance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-6",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Investment Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-6",
                FundingCap = 3500,
                Duration = 18,
                Title = "Providing Financial Services: Investment Operations - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-3-3",
                FundingCap = 2000,
                Duration = 12,
                Title = "Providing Financial Services: Life, Pensions and Investments - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-3",
                FundingCap = 3000,
                Duration = 18,
                Title = "Providing Financial Services: Life, Pensions and Investments - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "455-2-7",
                FundingCap = 3500,
                Duration = 18,
                Title = "Providing Financial Services: Pensions Administration - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "440-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Providing Security Services: Providing Security Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "572-20-1",
                FundingCap = 3000,
                Duration = 12,
                Title = "Public Relations: Public Relations - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "73",
                FundingCap = 9000,
                Duration = 24,
                Title = "Public sector commercial professional - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "36",
                FundingCap = 3000,
                Duration = 12,
                Title = "Public service operational delivery officer - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "88",
                FundingCap = 27000,
                Duration = 48,
                Title = "Rail engineering advanced technician - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "90",
                FundingCap = 12000,
                Duration = 12,
                Title = "Rail engineering operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "89",
                FundingCap = 27000,
                Duration = 36,
                Title = "Rail engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "545-2-4",
                FundingCap = 12000,
                Duration = 36,
                Title = "Rail Infrastructure Engineering: Electrification - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "545-3-2",
                FundingCap = 4000,
                Duration = 18,
                Title = "Rail Infrastructure Engineering: Electrification Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "545-2-1",
                FundingCap = 12000,
                Duration = 36,
                Title = "Rail Infrastructure Engineering: Signalling - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "545-2-3",
                FundingCap = 12000,
                Duration = 36,
                Title = "Rail Infrastructure Engineering: Telecoms - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "545-2-2",
                FundingCap = 12000,
                Duration = 36,
                Title = "Rail Infrastructure Engineering: Track - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "545-3-1",
                FundingCap = 5000,
                Duration = 18,
                Title = "Rail Infrastructure Engineering: Track Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "188",
                FundingCap = 12000,
                Duration = 12,
                Title = "Rail Infrastructure Operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "429-3-5",
                FundingCap = 3500,
                Duration = 18,
                Title = "Rail Services: Control Room Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "429-3-2",
                FundingCap = 3000,
                Duration = 18,
                Title = "Rail Services: Driving - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "429-3-1",
                FundingCap = 3500,
                Duration = 12,
                Title = "Rail Services: Passenger Services - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "429-2-1",
                FundingCap = 1500,
                Duration = 18,
                Title = "Rail Services: Rail Supervision - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "429-3-4",
                FundingCap = 3500,
                Duration = 18,
                Title = "Rail Services: Shunting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "429-3-3",
                FundingCap = 3500,
                Duration = 18,
                Title = "Rail Services: Signal Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "24",
                FundingCap = 18000,
                Duration = 36,
                Title = "Railway engineering design technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "217",
                FundingCap = 5000,
                Duration = 12,
                Title = "Recruitment Consultant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "218",
                FundingCap = 5000,
                Duration = 12,
                Title = "Recruitment Resourcer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "594-2-1",
                FundingCap = 1500,
                Duration = 18,
                Title = "Recruitment: Recruitment - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "594-20-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Recruitment: Recruitment - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "594-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Recruitment: Recruitment Resourcing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "49",
                FundingCap = 18000,
                Duration = 36,
                Title = "Refrigeration air conditioning and heat pump engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "514-3-2",
                FundingCap = 4000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Air Conditioning - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "514-2-3",
                FundingCap = 6000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Air Conditioning - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "514-2-4",
                FundingCap = 6000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Air Conditioning Service and Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "514-3-1",
                FundingCap = 4000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Refrigeration - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "514-2-1",
                FundingCap = 6000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Refrigeration - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "514-2-2",
                FundingCap = 6000,
                Duration = 24,
                Title = "Refrigeration and Air Conditioning: Refrigeration Service and Maintenance - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "170",
                FundingCap = 27000,
                Duration = 48,
                Title = "Registered Nurse - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "7",
                FundingCap = 27000,
                Duration = 48,
                Title = "Relationship manager(banking) - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "147",
                FundingCap = 6000,
                Duration = 18,
                Title = "Retail manager - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "140",
                FundingCap = 5000,
                Duration = 12,
                Title = "Retail team leader - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "443-2-3",
                FundingCap = 1500,
                Duration = 15,
                Title = "Retail: Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "443-2-4",
                FundingCap = 1500,
                Duration = 15,
                Title = "Retail: Multi-channel retail - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "443-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Retail: Retail - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "443-2-1",
                FundingCap = 1500,
                Duration = 15,
                Title = "Retail: Sales Professional - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "443-3-2",
                FundingCap = 1500,
                Duration = 12,
                Title = "Retail: Specialist - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "443-2-2",
                FundingCap = 1500,
                Duration = 15,
                Title = "Retail: Visual Merchandising - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "101",
                FundingCap = 5000,
                Duration = 12,
                Title = "Retailer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "485-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Sales and Telesales: Sales and Telesales - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "485-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = "Sales and Telesales: Sales and Telesales - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "45",
                FundingCap = 27000,
                Duration = 36,
                Title = "Science industry maintenance technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "15",
                FundingCap = 27000,
                Duration = 18,
                Title = "Science manufacturing technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "421-3-1",
                FundingCap = 2500,
                Duration = 24,
                Title = "Security Systems: Security Systems - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "421-2-1",
                FundingCap = 6000,
                Duration = 24,
                Title = "Security Systems: Security Systems - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "139",
                FundingCap = 5000,
                Duration = 12,
                Title = "Senior chef production cooking - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "144",
                FundingCap = 27000,
                Duration = 36,
                Title = "Senior compliance / risk specialist - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "31",
                FundingCap = 9000,
                Duration = 12,
                Title = "Senior financial services customer adviser - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "151",
                FundingCap = 3000,
                Duration = 18,
                Title = "Senior healthcare support worker - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "66",
                FundingCap = 9000,
                Duration = 18,
                Title = "Senior housing / property management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "205",
                FundingCap = 21000,
                Duration = 42,
                Title = "Senior Insurance Professional - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "416-3-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Signmaking: Signmaker/Installer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "416-2-1",
                FundingCap = 12000,
                Duration = 24,
                Title = "Signmaking: Signmaker/Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "579-2-2",
                FundingCap = 4000,
                Duration = 18,
                Title = "Social Media and Digital Marketing: Digital Marketing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "579-20-1",
                FundingCap = 4000,
                Duration = 12,
                Title = "Social Media and Digital Marketing: Digital Marketing - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "579-2-1",
                FundingCap = 3000,
                Duration = 18,
                Title = "Social Media and Digital Marketing: Social Media - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "2",
                FundingCap = 18000,
                Duration = 24,
                Title = "Software developer - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "154",
                FundingCap = 15000,
                Duration = 12,
                Title = "Software Development Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "91",
                FundingCap = 18000,
                Duration = 24,
                Title = "Software tester - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "43",
                FundingCap = 27000,
                Duration = 60,
                Title = "Solicitor - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "158",
                FundingCap = 4000,
                Duration = 24,
                Title = "Spectacle Maker - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "458-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Spectator Safety: Advanced Spectator Safety - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "458-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Spectator Safety: Spectator Safety - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "465-2-1",
                FundingCap = 5000,
                Duration = 18,
                Title = "Sporting Excellence: Sporting Excellence Pathway One - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "465-2-2",
                FundingCap = 2500,
                Duration = 12,
                Title = "Sporting Excellence: Sporting Excellence Pathway Two - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "467-2-1",
                FundingCap = 2000,
                Duration = 18,
                Title = "Sports development: Sports Development - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "145",
                FundingCap = 5000,
                Duration = 18,
                Title = "Sports turf operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "566-21-2",
                FundingCap = 6000,
                Duration = 12,
                Title = "Supply Chain Management: International Supply Chain Manager - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "566-2-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Supply Chain Management: Supply Chain Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "566-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Supply Chain Management: Supply Chain Operations - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "566-21-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Supply Chain Management: Supply Chain Specialist - Level 5",
                Level = 5
            },
            new ApprenticeshipCourse
            {
                Id = "109",
                FundingCap = 3000,
                Duration = 12,
                Title = "Supply chain operator - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "209",
                FundingCap = 15000,
                Duration = 30,
                Title = "Supply Chain Practitioner - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "111",
                FundingCap = 3000,
                Duration = 12,
                Title = "Supply chain warehouse operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "613-2-1",
                FundingCap = 3000,
                Duration = 12,
                Title =
                    "Supporting Teaching and Learning in Physical Education and School Sport: Supporting Teaching and Learning in Physical Education and School Sport - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "420-3-1",
                FundingCap = 2000,
                Duration = 12,
                Title =
                    "Supporting Teaching and Learning in Schools: Supporting Teaching and Learning in Schools - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "420-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title =
                    "Supporting Teaching and Learning in Schools: Supporting Teaching and Learning in Schools - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "51",
                FundingCap = 9000,
                Duration = 24,
                Title = "Surveying technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "532-2-1",
                FundingCap = 6000,
                Duration = 24,
                Title = "Surveying: Surveying - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "116",
                FundingCap = 27000,
                Duration = 36,
                Title = "Survival equipment fitter - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "505-3-1",
                FundingCap = 1500,
                Duration = 22,
                Title = "Sustainable Resource Management: Sustainable Resource Management - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "505-2-1",
                FundingCap = 2000,
                Duration = 30,
                Title = "Sustainable Resource Management: Sustainable Resource Management - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "618-20-1",
                FundingCap = 3500,
                Duration = 12,
                Title =
                    "Sustainable Resource Operations and Management: Sustainable Resource Operations and Management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "52",
                FundingCap = 27000,
                Duration = 36,
                Title = "Systems engineering masters level - Level 7",
                Level = 7
            },
            new ApprenticeshipCourse
            {
                Id = "203",
                FundingCap = 9000,
                Duration = 12,
                Title = "Teacher - Level 6",
                Level = 6
            },
            new ApprenticeshipCourse
            {
                Id = "105",
                FundingCap = 5000,
                Duration = 12,
                Title = "Team leader / supervisor - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "494-3-1",
                FundingCap = 2000,
                Duration = 15,
                Title = "Technical Theatre, Lighting, Sound and Stage: Lighting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "494-2-1",
                FundingCap = 2500,
                Duration = 18,
                Title = "Technical Theatre, Lighting, Sound and Stage: Lighting - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "494-3-2",
                FundingCap = 2000,
                Duration = 15,
                Title = "Technical Theatre, Lighting, Sound and Stage: Sound - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "494-2-2",
                FundingCap = 2500,
                Duration = 18,
                Title = "Technical Theatre, Lighting, Sound and Stage: Sound - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "494-3-3",
                FundingCap = 2000,
                Duration = 15,
                Title = "Technical Theatre, Lighting, Sound and Stage: Stage - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "494-2-3",
                FundingCap = 2500,
                Duration = 18,
                Title = "Technical Theatre, Lighting, Sound and Stage: Stage - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "446-2-1",
                FundingCap = 9000,
                Duration = 24,
                Title = "The Gas Industry: Gas Fired Wet Central Heating - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "446-2-2",
                FundingCap = 12000,
                Duration = 12,
                Title = "The Gas Industry: Gas Heating and Energy Efficiency - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "446-3-1",
                FundingCap = 3500,
                Duration = 12,
                Title = "The Gas Industry: Network Construction Operations(Gas) - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "446-2-3",
                FundingCap = 0,
                Duration = 0,
                Title = "The Gas Industry: Network Construction Operations(Gas) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "427-3-1",
                FundingCap = 6000,
                Duration = 30,
                Title = "The Power Sector: Power Transmission and Distribution - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "535-3-2",
                FundingCap = 3000,
                Duration = 24,
                Title = "The Water Industry: Sewerage Operations and Maintenance - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "535-20-1",
                FundingCap = 9000,
                Duration = 12,
                Title = "The Water Industry: Utilities Network Planning and Management - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "535-3-1",
                FundingCap = 3500,
                Duration = 16,
                Title = "The Water Industry: Water Industry - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "535-2-1",
                FundingCap = 12000,
                Duration = 20,
                Title = "The Water Industry: Water Industry - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "412-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Traffic Office: Traffic Office Clerk - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "412-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Traffic Office: Traffic Office Manager - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "100",
                FundingCap = 12000,
                Duration = 36,
                Title = "Transport planning technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "120",
                FundingCap = 9000,
                Duration = 12,
                Title = "Travel consultant - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "408-3-3",
                FundingCap = 2000,
                Duration = 12,
                Title = "Travel Services: Tour Operators - Field Staff - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "408-2-3",
                FundingCap = 3000,
                Duration = 12,
                Title = "Travel Services: Tour Operators - Field Staff - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "408-3-2",
                FundingCap = 2000,
                Duration = 12,
                Title = "Travel Services: Tour Operators - Head Office - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "408-2-2",
                FundingCap = 3000,
                Duration = 12,
                Title = "Travel Services: Tour Operators - Head Office - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "531-3-1",
                FundingCap = 1500,
                Duration = 18,
                Title = "Trees and Timber: Trees and Timber - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "531-2-1",
                FundingCap = 1500,
                Duration = 20,
                Title = "Trees and Timber: Trees and Timber - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "155",
                FundingCap = 15000,
                Duration = 12,
                Title = "Unified Communications Technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "81",
                FundingCap = 18000,
                Duration = 24,
                Title = "Unified communications trouble shooter - Level 4",
                Level = 4
            },
            new ApprenticeshipCourse
            {
                Id = "53",
                FundingCap = 27000,
                Duration = 48,
                Title = "Utilities engineering technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-5",
                FundingCap = 3000,
                Duration = 18,
                Title = "Vehicle Body and Paint: Automotive Glazing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-2",
                FundingCap = 4000,
                Duration = 24,
                Title = "Vehicle Body and Paint: Body Building - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "434-2-2",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Body and Paint: Body Building - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-4",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Body and Paint: Body Refinishing - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "434-2-4",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Body and Paint: Body Refinishing - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-3",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Body and Paint: Body Repair - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "434-2-3",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Body and Paint: Body Repair - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Body and Paint: Mechanical, Electrical and Trim - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "434-2-1",
                FundingCap = 5000,
                Duration = 18,
                Title = "Vehicle Body and Paint: Mechanical, Electrical and Trim - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-2-5",
                FundingCap = 2500,
                Duration = 12,
                Title = "Vehicle Body and Paint: Vehicle Damage Assessment - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "434-3-6",
                FundingCap = 3000,
                Duration = 12,
                Title = "Vehicle Body and Paint: Windscreen Repair - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "437-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Fitting: Fast Fit - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "437-2-1",
                FundingCap = 5000,
                Duration = 18,
                Title = "Vehicle Fitting: Fast Fit - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "437-3-2",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Fitting: Specialist Tyre Fitting - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-4",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Auto Electrics/Mobile Electrics - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-4",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Auto Electrics/Mobile Electrics - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-7",
                FundingCap = 3000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Caravan and Motorhome - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-6",
                FundingCap = 3500,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Caravan and Motorhome - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-2",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Heavy Vehicle - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-2",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Heavy Vehicle - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-5",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Heavy Vehicle Trailer - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-6",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Lift Truck - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-5",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Lift Truck - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-1",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Light Vehicle - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-1",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Light Vehicle - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "436-3-3",
                FundingCap = 6000,
                Duration = 24,
                Title = "Vehicle Maintenance and Repair: Motorcycle - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "436-2-3",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Maintenance and Repair: Motorcycle - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "433-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Parts: Vehicle Parts - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "433-2-1",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Parts: Vehicle Parts - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "634-3-1",
                FundingCap = 5000,
                Duration = 24,
                Title = "Vehicle Restoration: Vehicle Restoration - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "634-2-1",
                FundingCap = 6000,
                Duration = 18,
                Title = "Vehicle Restoration: Vehicle Restoration - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "435-3-1",
                FundingCap = 3000,
                Duration = 24,
                Title = "Vehicle Sales: Vehicle Sales - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "435-2-1",
                FundingCap = 4000,
                Duration = 18,
                Title = "Vehicle Sales: Vehicle Sales - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "529-2-2",
                FundingCap = 5000,
                Duration = 36,
                Title = "Veterinary Nursing: Equine - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "529-2-1",
                FundingCap = 6000,
                Duration = 36,
                Title = "Veterinary Nursing: Small Animal - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "414-2-1",
                FundingCap = 2000,
                Duration = 12,
                Title = "Warehousing and Storage: Senior Warehouse Person/Team Leader - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "414-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Warehousing and Storage: Warehouse Operative - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "27",
                FundingCap = 12000,
                Duration = 48,
                Title = "Water process technician - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "94",
                FundingCap = 9000,
                Duration = 18,
                Title = "Welding - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "95",
                FundingCap = 12000,
                Duration = 38,
                Title = "Welding - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "32",
                FundingCap = 9000,
                Duration = 18,
                Title = "Workplace pensions(administrator or consultant) - Level 3",
                Level = 3
            },
            new ApprenticeshipCourse
            {
                Id = "447-3-1",
                FundingCap = 1500,
                Duration = 12,
                Title = "Youth Work: Youth Work - Level 2",
                Level = 2
            },
            new ApprenticeshipCourse
            {
                Id = "447-2-1",
                FundingCap = 2500,
                Duration = 12,
                Title = "Youth Work: Youth Work - Level 3",
                Level = 3
            }
        };
        public List<ApprenticeshipCourse> GetApprenticeshipCourses()
        {
            return Courses;
        }

        public ApprenticeshipCourse GetApprenticeshipCourse(string courseId)
        {
            return Courses.FirstOrDefault(course => course.Id == courseId);
        }
    }

    public interface IApprenticeshipCourseService
    {
        List<ApprenticeshipCourse> GetApprenticeshipCourses();

        ApprenticeshipCourse GetApprenticeshipCourse(string courseId);
    }
}
