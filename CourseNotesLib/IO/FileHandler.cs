//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UtilityLibrary.IO;
using CourseNotesLib.Core;
using CourseNotesLib.Domain_Classes;
using UtilityLibrary.DirectoryUtility;
using UtilityLibrary.Strings;

namespace CourseNotesLib.IO
{
    //I didn't put too much effort into file loading / creating a separate DAL since I was more interested in using Func/Actions/Events interally & between the display & BLL layer
    //so this file handler is pretty clunky & makes some assumptions about the correctness of the file it loads (although I wrap it in try/catches at least so it doesn't break the program)
    internal class FileHandler
    {
        #region Fields

        private const string ChildrenId_tag = "ChildrenId";

        private const string Course_Tag = "Course";

        //Domain object / list tags
        private const string CourseList_Tag = "Courses";

        //Attribute tags
        private const string Id_Tag = "Id";

        private const string Link_Tag = "Link";

        private const string Module_Tag = "Module";

        private const string ModuleList_Tag = "Modules";

        private const string Note_Tag = "Note";

        private const string NoteList_Tag = "Notes";

        private const string ParentId_Tag = "ParentId";

        //Meta tags
        private const string Start_Tag = "CourseNotes";
        private const string Tag_Tag = "Tag";
        private const string TagList_Tag = "Tags";
        private readonly Repository repository;

        #endregion Fields

        #region Constructors

        internal FileHandler(Repository repository)
        {
            this.repository = repository;
        }

        #endregion Constructors

        #region Methods

        public bool Load(FileInfo arg)
        {
            try
            {
                return LoadFromFile(arg.FullName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                return false;
            }
        }

        
        public bool Save(FileInfo arg)
        {
            return SaveAs(arg.Directory, arg.NameWithoutFilePath()).Exists;
        }

        public FileInfo SaveAs(DirectoryInfo aDirectory, string fileName)
        {
            //Create the file path
            string _filePath = Path.Combine(aDirectory.FullName, fileName + ".xml");

            //Start the xml writer and write the document start
            using(XmlWriter _xmlWriter = WriteDocumentStart(_filePath))
            {
                //Write all tags
                WriteTags(_xmlWriter);

                //Write all courses
                WriteCourses(_xmlWriter);

                //Write all modules
                WriteModules(_xmlWriter);

                //Write all notes
                WriteNotes(_xmlWriter);


                //Write the document end
                XmlMethods.WriteDocumentEnd(_xmlWriter);
            }

            //return the FileInfo
            return new FileInfo(_filePath);
        }



        private bool LoadFromFile(string aFullPath)
        {

            try
            {
                //Create a XmlDocument that we can use to brows the nodes
                XmlDocument _xmlDoc = new XmlDocument();
                //Load it via the provided full path
                _xmlDoc.Load(aFullPath);

                //Load all tags / courses etc via helper methods
                var _tags = LoadTags(_xmlDoc);

                var _courses = LoadCourses(_xmlDoc);

                var _modules = LoadModules(_xmlDoc);

                var _notes = LoadNotes(_xmlDoc);

                //If any failed w/ loading we return false
                if(_tags == null || _courses == null || _modules == null || _notes == null)
                {
                    return false;
                }

                //Otherwise we return the load / validator from the repository
                return repository.Load(_courses, _modules, _notes, _tags);
            }
            catch(Exception _e)
            {
                throw new FileLoadException("Failed with loading from Xml-file.", _e);
            }
        }

        private List<Course> LoadCourses(XmlDocument xmlDoc)
        {
            try
            { 
                //Attempts to load all courses via a Xpath based on the Start/CourseList/Course tags
                var _courseNodes = xmlDoc.SelectNodes($"/{Start_Tag}/{CourseList_Tag}/{Course_Tag}");

                if(_courseNodes == null)
                    return null;

                List<Course> _loadedCourses = new List<Course>();

                //For each valid node found with the Xpath we attempt to extract our attributes and text into a new course object
                foreach(XmlNode _node in _courseNodes)
                {
                    string _courseText = _node.InnerText;
                    string _courseLink = _node.Attributes[Link_Tag].Value;
                    bool _parsedId = int.TryParse(_node.Attributes[Id_Tag].Value, out int _courseId);

                    if(_parsedId)
                        _loadedCourses.Add(new Course(_courseId, _courseText, _courseLink));
                }

                return _loadedCourses;
            }
            catch(Exception _e)
            {
                Console.WriteLine(_e);
                throw;
            }
        }

        private List<Module> LoadModules(XmlDocument xmlDoc)
        {
            //Follows the same pattern as LoadCourses, get the nodes and attempt to extract what we need from them
            try
            {
                var _moduleNodes = xmlDoc.SelectNodes($"/{Start_Tag}/{ModuleList_Tag}/{Module_Tag}");

                if(_moduleNodes == null)
                    return null;

                List<Module> _loadedModules = new List<Module>();

                foreach(XmlNode _node in _moduleNodes)
                {
                    string _moduleText = _node.InnerText;
                    bool _parsedModuleId = int.TryParse(_node.Attributes[Id_Tag].Value, out int _moduleId);
                    bool _parsedCourseId = int.TryParse(_node.Attributes[ParentId_Tag].Value, out int _courseId);

                    if(_parsedModuleId && _parsedCourseId)
                    {
                        _loadedModules.Add(new Module(_moduleId, _courseId, _moduleText));
                    }
                }

                return _loadedModules;
            }
            catch(Exception _e)
            {
                Console.WriteLine(_e);
                throw;
            }
        }

        private List<Note> LoadNotes(XmlDocument xmlDoc)
        {
            try
            {
                var _noteNodes = xmlDoc.SelectNodes($"/{Start_Tag}/{NoteList_Tag}/{Note_Tag}");

                if(_noteNodes == null)
                    return null;

                List<Note> _loadedNotes = new List<Note>();

                foreach(XmlNode _node in _noteNodes)
                {
                    string _noteText = _node.InnerText;
                    string _noteLink = _node.Attributes[Link_Tag].Value;
                    bool _parsedNoteId = int.TryParse(_node.Attributes[Id_Tag].Value, out int _noteId);
                    bool _parsedCourseId = int.TryParse(_node.Attributes[ParentId_Tag].Value, out int _moduleId);

                    //Unlike the other domain objects we need to perform an extra step of splitting the saved string back into ints
                    string _tagIds = _node.Attributes[ChildrenId_tag].Value;
                    //This method of splitting is a bit null-unsafe but it's valid for the way we saved it in this class
                    var _tags = Delimiters.SplitString(InternalConstants.DelimitChar, _tagIds).ConvertAll(t => int.Parse(t));

                    if(_parsedNoteId && _parsedCourseId)
                    {
                        Note _loadedNote = new Note(_noteId, _moduleId, _noteText, _noteLink);
                        if(_tags.Count > 0)
                            _loadedNote.SetTags(_tags);
                        _loadedNotes.Add(_loadedNote);
                    }
                }

                return _loadedNotes;
            }
            catch(Exception _e)
            {
                Console.WriteLine(_e);
                throw;
            }
        }

        private List<Tag> LoadTags(XmlDocument xmlDoc)
        {
            //Standard load like for Courses & Modules
            try
            {
                var _tagNodes = xmlDoc.SelectNodes($"/{Start_Tag}/{TagList_Tag}/{Tag_Tag}");

                if(_tagNodes == null)
                    return null;

                List<Tag> _loadedTags = new List<Tag>();

                foreach(XmlNode _node in _tagNodes)
                {
                    string _tagText = _node.InnerText;
                    bool _parsedId = int.TryParse(_node.Attributes[Id_Tag].Value, out int _tagId);

                    if(_parsedId)
                        _loadedTags.Add(new Tag(_tagId, _tagText));
                }

                return _loadedTags;
            }
            catch(Exception _e)
            {
                Console.WriteLine(_e);
                throw;
            }
        }

        private XmlWriter WriteDocumentStart(string aPath)
        {
            //Make the document a bit more readable for a human via indents
            XmlWriterSettings _settings = new XmlWriterSettings();
            _settings.Indent = true;
            _settings.IndentChars = "\t";

            //Create the writer and apply the settigns
            XmlWriter _xmlWriter = XmlWriter.Create(aPath, _settings);

            //Write the start tags and return the writer
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement(Start_Tag);
            return _xmlWriter;
        }

        private void WriteCourses(XmlWriter xmlWriter)
        {
            //Select the names of the courses to one list
            List<string> _courses = repository.Courses.Select(t => t.Name).ToList();
            //With the way my old Xml helper class' list writer works we need to extract all attributes into lists of arrays of tag / value combinations
            //this is because each element in the courses list corresponds to an array of attributes (for courses: Id + Link)
            //By saving them with a tag we can access them like a dictionary on load, although this means we have to save empty values too (ie Link = "") to avoid exceptions when accessing
            List<(string, string)[]> _attributes = repository.Courses.Select(course =>
                    new[]
                    {
                        (Id_Tag, course.Id.ToString()),
                        (Link_Tag, course.Link)
                    })
                .ToList();

            //Write the list with its tags and all its elements with their tag
            XmlMethods.WriteStringList(xmlWriter, CourseList_Tag, Course_Tag, _courses, _attributes);
        }

        private void WriteModules(XmlWriter xmlWriter)
        {
            //See WriteCourses for detailed explanation
            List<string> _modules = repository.Modules.Select(t => t.ModuleName).ToList();
            List<(string, string)[]> _attributes = repository.Modules.Select(module =>
                new[]
                {
                    (Id_Tag, module.Id.ToString()),
                    (ParentId_Tag, module.CourseId.ToString())
                }).ToList();

            XmlMethods.WriteStringList(xmlWriter, ModuleList_Tag, Module_Tag, _modules, _attributes);
        }

        private void WriteNotes(XmlWriter xmlWriter)
        {
            //See WriteCourses for detailed explanation
            List<string> _notes = repository.Notes.Select(t => t.Text).ToList();
            List<(string, string)[]> _attributes = repository.Notes.Select(note =>
                new[]
                {
                    (Id_Tag, note.Id.ToString()),
                    (ParentId_Tag, note.ModuleId.ToString()),
                    (ChildrenId_tag, string.Join(";",note.TagIds)), //We create a delimited string for the ids that we can later convert back to ints by splitting & parsing
                    (Link_Tag, note.Link)
                }).ToList();

            XmlMethods.WriteStringList(xmlWriter, NoteList_Tag, Note_Tag, _notes, _attributes);
        }

        private void WriteTags(XmlWriter xmlWriter)
        {
            //Slightly clunky linq & casts but I'm reusing a pretty old XML helper class I wrote so I kinda have to make it fit
            List<string> _tags = repository.Tags.Select(t => t.TagText).ToList();
            List<(string, string)[]> _attributes = repository.Tags.Select(repositoryTag => new[] { (Id_Tag, repositoryTag.Id.ToString()) }).ToList();
            XmlMethods.WriteStringList(xmlWriter, TagList_Tag, Tag_Tag, _tags, _attributes);
        }

        #endregion Methods
    }
}