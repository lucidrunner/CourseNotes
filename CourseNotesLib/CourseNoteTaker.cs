//Björn Rundquist 16/11-2020
using CourseNotesLib.Core;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.IO;

namespace CourseNotesLib
{
    /// <summary>
    /// The main class of the library. Takes a INoteTakerInterface on creation and sets up a series of func/action/event binding based on the implementers listed in the INoteTaker.
    /// </summary>
    public class CourseNoteTaker
    {
        private readonly CourseHandler courseHandler;
        private readonly ModuleHandler moduleHandler;
        private readonly NotesHandler notesHandler;
        private readonly TagsHandler tagsHandler;
        private readonly SearchHandler searchHandler;
        private readonly PresentationConverter presentationConverter;
        private readonly FileHandler fileHandler;
        private readonly Repository repository = new Repository();
        
        
        public CourseNoteTaker(INoteTakerInterface aNoteTakerInterface)
        {
            //Initialize all of our components, passing along their respective implementers
            courseHandler = new CourseHandler(aNoteTakerInterface.CourseCreator, aNoteTakerInterface.CourseDisplay, repository);
            moduleHandler = new ModuleHandler(aNoteTakerInterface.ModuleDisplay, repository);
            notesHandler = new NotesHandler(aNoteTakerInterface.NotesDisplay, aNoteTakerInterface.NotesCreator, aNoteTakerInterface.NoteViewer, aNoteTakerInterface.NotePrinter, repository);
            tagsHandler = new TagsHandler(repository);
            searchHandler = new SearchHandler(aNoteTakerInterface.DisplayFilterer);
            //Initializes a presentation converter, which takes domain interfaces and converts them to presentable formats
            presentationConverter = new PresentationConverter(repository);
            //Initialize our FileHandler, I went with a simpler FileHandler / repository pattern for this assignment rather than creating a full blown DAL
            fileHandler = new FileHandler(repository);

            //Bind the internal connections between our different componentss
            BindInternalDelegates(aNoteTakerInterface);

            //Bind the presentation converter to the correct implementers
            LinkPresentationConverter(aNoteTakerInterface);

        }

        private void BindInternalDelegates(INoteTakerInterface aNoteTakerInterface)
        {
            //Course Handler
            courseHandler.AddModules += moduleHandler.AddModules;
            courseHandler.DisplayModules += moduleHandler.DisplayModules;

            //Module Handler
            moduleHandler.DisplayNotes += notesHandler.DisplayNotes;
            moduleHandler.DisplayNotesForAllModules += searchHandler.DisplayForAll;

            //Notes Handler
            notesHandler.GetDisplayedModuleId += moduleHandler.GetCurrentSingleId;
            notesHandler.GetTagsFromInput += tagsHandler.GetTagIds;
            notesHandler.TagIdConverter += tagsHandler.ConvertTagIds;
            notesHandler.NoteModified += (sender, args) => moduleHandler.RefreshModuleNotesDisplay();

            //Search Handler
            searchHandler.SearchUpdated += moduleHandler.RefreshModuleNotesDisplay;
            searchHandler.TagIdConverter += tagsHandler.GetTagStrings;
            notesHandler.NoteFilterPredicate += searchHandler.PerformWordSearch;
            notesHandler.NoteFilterPredicate += searchHandler.PerformTagSearch;

            //Converter
            presentationConverter.TagIdConverter += tagsHandler.ConvertTagIds;

            //File Handler
            aNoteTakerInterface.SaveToFile += fileHandler.Save;
            aNoteTakerInterface.LoadFromFile += fileHandler.Load;
            aNoteTakerInterface.SaveAs += fileHandler.SaveAs;

            //Repository
            repository.RepositoryLoaded += delegate
            {
                //Begin by resetting the display so we don't get any errors when loading the new repository
                aNoteTakerInterface.ResetDisplay?.Invoke();
                //Then display the course list
                courseHandler.UpdateCourseDisplayList();
            };
        }

        private void LinkPresentationConverter(INoteTakerInterface aNoteTakerInterface)
        {
            //We could also have passed the NoteTaker Interface and let the converter internally check all subinterfaces for if they implements a presentation converter interface
            //However, I kept it a bit simple and only linked the ones I knew I needed it for
            presentationConverter.LinkPresentationComponent(aNoteTakerInterface.NotePrinter);
            presentationConverter.LinkPresentationComponent(aNoteTakerInterface.NotesDisplay);
            presentationConverter.LinkPresentationComponent(aNoteTakerInterface.NoteViewer);
        }
    }
}