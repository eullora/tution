using System.Collections.ObjectModel;

namespace Zeeble.Mobile.ViewModels
{
    public class AttendanceGroupModel
    {
        public string Title { get; set; }
        public int Percent { get; set; }    
    }

    public class AttendanceViewModel
    {
        public ObservableCollection<AttendanceGroupModel> Data { get; set; }        

        public AttendanceViewModel()
        {
            Data = new ObservableCollection<AttendanceGroupModel>
            {
                new AttendanceGroupModel { Title = "Present",  Percent = 70 },
                new AttendanceGroupModel { Title = "Absent",  Percent = 30 }
            };
        }
    }
}
