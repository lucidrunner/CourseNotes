using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CourseNotesLib;
using CourseNotesLib.Core;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Domain_Interfaces;
using CourseNotesLib.Utility;
using CourseNotesLib.Utility.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CourseNotesLibTests
{
    /// <summary>
    /// Some unit testing I ran to prep for assignment 4 + just to test the different components
    /// </summary>

    [TestClass]
    public class HandlerTests
    {
        private Repository repository = new Repository();


        [TestMethod]
        public void DisplayNotesTest()
        {
            FillRepository();
            DummyNotesDisplay _display = new DummyNotesDisplay();
            NotesHandler _notesHandler = new NotesHandler(_display, null, null, null, repository);
            
            //Attempt to display all notes for all modules
            _notesHandler.DisplayNotes(repository.Modules.Select(t => t.Id));


            PrintCollection("Found result notes are ", _display.Result.Select(t => t.Text));
            //Assert that we've by selecting all modules also have gotten all notes
            Assert.AreEqual(_display.Result.Count, repository.Notes.Count);


            //Assert that by adding a predicate to the handler it is executed
            _notesHandler.NoteFilterPredicate += note => note.Link.StartsWith("www");
            _notesHandler.DisplayNotes(repository.Modules.Select(t => t.Id));
            PrintCollection("Found result notes with predicate for www are ", _display.Result.Select(t => t.Text));
            Assert.IsTrue(_display.Result.Select(t => t.Link).All(s => s.StartsWith("www")));

        }

        [TestMethod]
        public void RemoveNoteTest()
        {
            FillRepository();
            DummyNotesDisplay _display = new DummyNotesDisplay();
            NotesHandler _notesHandler = new NotesHandler(_display, null, null, null, repository);
            //Remove the note with Id 0, this func will have been automatically connected by the handler on setup
           // Assert.IsTrue(_display.?.Invoke(0) ?? false); previously this was a func which allowed me to read the result
            PrintCollection("Notes after deletion of id 0", repository.Notes.Select(t => t.Text + ", ID: " + t.Id));

            Assert.IsNull(repository.Notes.FirstOrDefault(t => t.Id == 0));

        }

        [TestMethod]
        public void GetModuleIdsTest()
        {
            FillRepository();
            DummyModuleDisplay _display = new DummyModuleDisplay(repository.Modules.Select(t => t.Id));
            ModuleHandler _moduleHandler = new ModuleHandler(_display, repository);
            
            //Assert that with the current selection, single ID return null
            Assert.IsNull(_moduleHandler.GetCurrentSingleId());

            //Set a single item as our selected id and assert that the single Id getter is working
            _display = new DummyModuleDisplay(new []{5});
            _moduleHandler = new ModuleHandler(_display, repository);
            Assert.IsTrue(_moduleHandler.GetCurrentSingleId().HasValue);
            Assert.IsTrue(_moduleHandler.GetCurrentSingleId().Value == 5);
        }

        [TestMethod]
        public void GetTagStringTest()
        {
            FillRepository();
            TagsHandler _tagsHandler = new TagsHandler(repository);
            var _result = _tagsHandler.ConvertTagIds(new[] {0, 2});

            
            
            Console.WriteLine("Result: " + _result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(_result));
            string _expected = repository.Tags.First(t => t.Id == 0).TagText + InternalConstants.DelimitChar[0] + repository.Tags.First(t => t.Id == 2).TagText;
            
            Console.WriteLine("Expected: " + _expected);
            Assert.IsTrue(_result.Equals(_expected));
        }

        [TestMethod]
        public void GetTagIdsTest()
        {
            FillRepository();
            TagsHandler _tagsHandler = new TagsHandler(repository);

            var _result = _tagsHandler.GetTagIds("Tag1;Tag2;Tag4").ToList();

            //Assert that the last tag has been added to the repository
            Assert.IsTrue(repository.Tags.Count == 4);

            //Assert that the last added tag has the new Id 3
            PrintCollection("Returned tag Ids", _result.Select(t => t.ToString()));
            int _expectedId = 3;
            Assert.IsTrue(_result[2] == _expectedId);
            //and the correct name based on the passed in list
            string _expectedName = "Tag4";
            Assert.IsTrue(repository.Tags.First(t => t.Id == _expectedId).TagText.Equals(_expectedName));
        }

        //TODO Incorrect tests for delimiter here too btw

        [TestMethod]
        public void TestSearchPredicates()
        {
            FillRepository();
            DummyDisplayFilterer displayFilterer = new DummyDisplayFilterer();
            SearchHandler searchHandler = new SearchHandler(displayFilterer);

            //Word and link search
            Note testNote = new Note(0, 0, "This text contains several words.", "");
            displayFilterer.WordSearch = "text";
            Assert.IsTrue(searchHandler.PerformWordSearch(testNote));
            displayFilterer.WordSearch = "www";
            Assert.IsFalse(searchHandler.PerformWordSearch(testNote));
            displayFilterer.IncludeLinks = true;
            Assert.IsFalse(searchHandler.PerformWordSearch(testNote));
            testNote = new Note(0, 0, "Some other words.", "www.test.com");
            Assert.IsTrue(searchHandler.PerformWordSearch(testNote));
            
            //Multi word search
            displayFilterer.WordSearch = "text;words;contains";
            Assert.IsTrue(searchHandler.PerformWordSearch(testNote));

            //Tag search
            //Set our dummy handler to always get all tags
            searchHandler.TagIdConverter += ints => repository.Tags.Select(t => t.TagText);

            displayFilterer.TagSearch = "1;2";
            Assert.IsTrue(searchHandler.PerformTagSearch(testNote));
            displayFilterer.TagSearch = "bogus";
            Assert.IsFalse(searchHandler.PerformTagSearch(testNote));

        }

        [TestMethod]
        public void NoteRemovalTest()
        {
            FillRepository();

            repository.RemoveNote(0);
            PrintCollection("Tags remaining after first removal", repository.Tags.Select(t => t.TagText));
            repository.RemoveNote(1);
            PrintCollection("Tags remaining after second removal", repository.Tags.Select(t => t.TagText));
            repository.RemoveNote(2);
            PrintCollection("Tags remaining after third removal", repository.Tags.Select(t => t.TagText));
            Assert.IsTrue(repository.Tags.Count == 0);
        }

        private void PrintCollection(string aStartMessage, IEnumerable<string> aToPrint)
        {
            Console.WriteLine(aStartMessage);
            foreach (string _s in aToPrint)
            {
                Console.WriteLine(_s);
            }

        }

        private void FillRepository()
        {
            repository = new Repository();

            var _tags = new[] {new Tag(0, "Tag1"),
                new Tag(1, "Tag2"),
                new Tag(2, "Tag3")};

            repository.Tags.AddRange(_tags);

            var _notes = new[] {new Note(0, 0, "Note1", ""),
                new Note(1, 0, "Note2", "www.link.com"),
                new Note(2, 1, "Note3", ""),
                new Note(3, 2, "Note4", "www.otherLink.com"), 
            };

            _notes[0].SetTags(_tags.Select(t => t.Id));

            _notes[1].SetTags(_tags.Select(t => t.Id).Where(id => id % 2 == 0));
            _notes[2].SetTags(_tags.Select(t => t.Id).Where(id => id % 2 != 0));

            repository.Notes.AddRange(_notes);

            var _modules = new[] {new Module(0, 0, "Module1"), new Module(1, 1, "Module2"), new Module(2, 1, "Module3")};

            repository.Modules.AddRange(_modules);

            var _courses = new[] {new Course(0, "Course1", "www.courseLink"), new Course(1, "Course2", "")};

            repository.Courses.AddRange(_courses);
        }


        private class DummyNotesDisplay: INotesDisplay
        {
            public List<INote> Result;
            public DummyNotesDisplay()
            {
                FillNotesList += notes => Result = notes.ToList();
            }


            public Action<int> ViewNote { get; set; }
            public Action<int> RemoveNote { get; set; }
            public Action<IEnumerable<INote>> FillNotesList { get; set; }
            public Action<IErrorMessage> DisplayErrorMessageAction { get;  }
        }

        private class DummyModuleDisplay : IModuleDisplay
        {
            public List<IModule> Result;
            public DummyModuleDisplay()
            {
                FillModuleListAction += modules => Result = modules.ToList();
            }
            public DummyModuleDisplay(IEnumerable<int> selectedModuleIds)
            {
                SelectedModuleIds = selectedModuleIds;
            }

            public Action<IErrorMessage> DisplayErrorMessageAction { get; }
            public Action<IEnumerable<IModule>> FillModuleListAction { get; set; }
            public IEnumerable<int> SelectedModuleIds { get; }
            public event EventHandler ModuleSelectionChanged;
        }

        private class DummyDisplayFilterer : IDisplayFilterer
        {
            public Action<IErrorMessage> DisplayErrorMessageAction { get; }
            public bool DisplayAll { get; set; }
            public string WordSearch { get; set; }
            public string TagSearch { get; set; }
            public bool IncludeLinks { get; set; }
            public event EventHandler FilterChanged;
        }
    }
}
