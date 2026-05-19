using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ViewModels;

namespace Documents
{
    public class PrescriptionPdfDocument : IDocument
    {
        private readonly PrescriptionVM _m;

        public PrescriptionPdfDocument(PrescriptionVM model)
        {
            _m = model;
        }

        public DocumentMetadata GetMetadata()
            => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(35);

                page.Content().Column(col =>
                {
                    // header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text($"{_m.DoctorName}   (Reg: {_m.RegistrationNumber})")
                                .Bold().FontSize(18);

                            left.Item().Text(_m.Qualification);

                            left.Item().PaddingTop(15);

                            left.Item().Text($"{_m.PatientId} - {_m.PatientName}")
                                .FontSize(16);

                            left.Item().Text(
                                $"Age: {_m.Age} | Gender: {_m.Gender} | Ph: {_m.PatientPhoneNo}");
                        });

                        row.RelativeItem().AlignRight().Column(right =>
                        {
                            right.Item().Text(_m.ClinicName)
                                .Bold().FontSize(16);

                            right.Item().Text(_m.ClinicAddress);

                            right.Item().PaddingTop(10);

                            right.Item().Text(
                                $"Date: {_m.VisitDateTime:dd/MM/yyyy}");

                            right.Item().Text(
                                $"Time: {_m.VisitDateTime:HH:mm}");
                        });
                    });

                    col.Item().PaddingVertical(15).LineHorizontal(1);

                    col.Item().Text("Symptoms").Bold();
                    col.Item().Text(_m.Symptoms);

                    col.Item().PaddingVertical(10).LineHorizontal(.5f);

                    col.Item().Text("Diagnosis").Bold();
                    col.Item().Text(_m.Diagnosis);

                    col.Item().PaddingTop(15);

                    col.Item().Text("Medicines").Bold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.RelativeColumn(3);
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("#").Bold();
                            h.Cell().Text("Medicine").Bold();
                            h.Cell().Text("Dosage").Bold();
                            h.Cell().Text("Frequency").Bold();
                            h.Cell().Text("Duration").Bold();
                            h.Cell().Text("Instructions").Bold();
                        });

                        int i = 1;

                        foreach (var m in _m.Medicines)
                        {
                            table.Cell().Text(i.ToString());
                            table.Cell().Text(m.MedicineName);
                            table.Cell().Text(m.Dosage);
                            table.Cell().Text(m.Frequency);
                            table.Cell().Text(m.Duration);
                            table.Cell().Text(m.Instructions);
                            i++;
                        }
                    });

                    col.Item().PaddingTop(15);

                    col.Item().Text("Notes").Bold();
                    col.Item().Text(_m.Notes);

                    col.Item().PaddingTop(70)
                        .AlignRight()
                        .Text("________________________");

                    col.Item().AlignRight()
                        .Text(_m.DoctorName).Bold();

                    col.Item().AlignRight()
                        .Text(_m.Qualification);

                    col.Item().PaddingVertical(15)
                        .LineHorizontal(.5f);

                    col.Item()
                        .AlignCenter()
                        .Text("This prescription is valid only with physician signature. Follow dosage carefully.");
                });
            });
        }
    }
}