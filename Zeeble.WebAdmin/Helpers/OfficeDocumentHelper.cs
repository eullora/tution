using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DrawBlip = DocumentFormat.OpenXml.Drawing;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Helpers
{
    public class OfficeDocumentHelper
    {
        public IEnumerable<QuestionModel> QuestionsFromWord(string filename)
        {
            var list = new List<QuestionModel>();

            try
            {
                using (var wDoc = WordprocessingDocument.Open(filename, false))
                {
                    var docPart = wDoc.MainDocumentPart;
                    var parts = wDoc.MainDocumentPart.Document.Descendants().FirstOrDefault();
                    if (parts != null)
                    {
                        foreach (var node in parts.ChildElements)
                        {
                            if (node is Table)
                            {
                                list.Add(ProcessTable((Table)node, docPart));
                            }
                        }
                    }
                }
            }
            catch { }

            return list;
        }

        public IEnumerable<QuizQuestionModel> QuizQuestionsFromWord(string filename)
        {
            var list = new List<QuizQuestionModel>();

            try
            {
                using (var wDoc = WordprocessingDocument.Open(filename, false))
                {
                    var docPart = wDoc.MainDocumentPart;
                    var parts = wDoc.MainDocumentPart.Document.Descendants().FirstOrDefault();
                    if (parts != null)
                    {
                        foreach (var node in parts.ChildElements)
                        {
                            if (node is Table)
                            {
                                list.Add(ProcessQuizTable((Table)node, docPart));
                            }
                        }
                    }
                }
            }
            catch { }

            return list;
        }

        private QuestionModel ProcessTable(Table node, MainDocumentPart docPart)
        {
            var rows = node.Descendants<TableRow>();
            var subjectCell = rows.ElementAt(0).Descendants<TableCell>().ElementAt(0);
            var imageCell = rows.ElementAt(1).Descendants<TableCell>().ElementAt(0);
            var answerCell = rows.ElementAt(2).Descendants<TableCell>().ElementAt(0);

            var image = imageCell.Descendants<DrawBlip.Blip>();
            var imgEmbedId = image.ElementAt(0).Embed;
            var imageData = (ImagePart)docPart.GetPartById(imgEmbedId);

            using var stream = imageData.GetStream();
            var byteStream = new byte[stream.Length];
            stream.Read(byteStream, 0, (int)stream.Length);
            var base64Image = Convert.ToBase64String(byteStream);

            return new QuestionModel
            {
                Id = Guid.NewGuid().ToString(),
                SubjectName = subjectCell.InnerText,
                ImageData = $"data:image/png;base64,{base64Image}",
                CorrectAnswer = answerCell.InnerText
            };
        }

        private QuizQuestionModel ProcessQuizTable(Table node, MainDocumentPart docPart)
        {
            var rows = node.Descendants<TableRow>();            
            var imageCell = rows.ElementAt(0).Descendants<TableCell>().ElementAt(0);
            var answerCell = rows.ElementAt(1).Descendants<TableCell>().ElementAt(0);

            var image = imageCell.Descendants<DrawBlip.Blip>();
            var imgEmbedId = image.ElementAt(0).Embed;
            var imageData = (ImagePart)docPart.GetPartById(imgEmbedId);

            using var stream = imageData.GetStream();
            var byteStream = new byte[stream.Length];
            stream.Read(byteStream, 0, (int)stream.Length);
            var base64Image = Convert.ToBase64String(byteStream);

            return new QuizQuestionModel
            {
                Id = Guid.NewGuid().ToString(),                
                ImageData = $"data:image/png;base64,{base64Image}",
                CorrectAnswer = answerCell.InnerText
            };
        }
    }
}
