namespace Zeeble.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
                        
            Routing.RegisterRoute("home", typeof(HomePage));
            Routing.RegisterRoute("documents", typeof(DocumentPage));            
            Routing.RegisterRoute("exams", typeof(OnlineExamPage));
            Routing.RegisterRoute("exam", typeof(ExamPage));
            Routing.RegisterRoute("quiz", typeof(QuizPage));
            Routing.RegisterRoute("videos", typeof(VideoPage));
            Routing.RegisterRoute("player", typeof(VideoPlayerPage));
            Routing.RegisterRoute("result", typeof(OnlineResultPage));
            Routing.RegisterRoute("attendance", typeof(AttendancePage));
            Routing.RegisterRoute("quizhome", typeof(QuizHomePage));            
            Routing.RegisterRoute("chapters", typeof(ChapterListPage));            
            Routing.RegisterRoute("fees", typeof(FeesPage));            
            Routing.RegisterRoute("flashcards", typeof(FlashCardsPage));            

        }      
    }
}
