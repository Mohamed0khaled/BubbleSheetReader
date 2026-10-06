# BubbleSheetReader

A lightweight ASP.NET Core Razor Pages application for reading and processing fixed-format bubble sheet forms. The project is designed around a calibrated template approach: it measures the darkness of each printed bubble against known coordinates, identifies selected options, and exports the valid daily load values to an Excel workbook.

This MVP focuses on the actual bubble-based recognition flow and the structured output format used in the repository's sample sheets. Handwriting fields are intentionally stubbed, and the project clearly labels them as non-recognized test values rather than pretending to perform real OCR.

## Features

- Upload a bubble sheet image in JPG, JPEG, PNG, or PDF format
- Detect fixed template coordinates using a calibrated reference size
- Read header values such as ID, truck number, shift, and crew
- Detect daily row equipment and material selections across 30 rows
- Generate a debug image showing recognized bubble areas
- Export valid rows into an Excel workbook with the required column mapping
- Surface warnings for empty, unclear, or multi-selected rows
- Keep the implementation simple, template-driven, and easy to extend

## Current project status

This repository implements the MVP workflow for the bubble detection use case:

- Bubble-based selection detection works on the provided test images
- Template coordinates are fixed to the sample sheet dimensions in the project
- Header recognition for ID and truck number is implemented
- Daily row recognition for equipment and material groups is implemented
- Handwriting recognition is a placeholder test stub, not a real OCR engine
- Excel export is included and uses the expected column names for downstream processing

## Tech stack

- ASP.NET Core Razor Pages
- .NET 10
- SixLabors.ImageSharp for image loading and pixel analysis
- ClosedXML for Excel workbook generation

## Project purpose

The workflow is intended to support a fixed-form sheet where the user uploads a scanned or photographed image, the app matches it to a known template, and the system extracts selected options from marked bubbles. The process is intentionally based on geometry and optical density rather than general computer-vision automation, which keeps the app fast and explainable for this specific form.

## How the app works

1. The uploaded image is loaded and preprocessed.
2. A template is created from known bubble positions for the sheet layout.
3. Each bubble region is evaluated by dark-pixel ratio.
4. The app decides whether the bubble is:
   - selected,
   - empty,
   - unclear,
   - or invalid due to multiple choices.
5. Header fields and daily rows are assembled into a result model.
6. A debug image is generated for visual verification.
7. Valid rows are exported to Excel.

## Template approach

This project uses a fixed-template model rather than flexible object detection. The template is defined in the codebase under the Templates folder and anchored to a known image size:

- reference width: 2481
- reference height: 3509
- template name: Template 1

The actual coordinates were calibrated against the sample images in the TestData folder, especially the scanned sheet and blank reference sheet. This gives the app stable bubble geometry and prevents it from guessing across different page sizes.

## Project structure

```text
BubbleSheetReader/
├── Models/
│   ├── BubbleSheetResult.cs
│   ├── BubbleSheetTemplate.cs
│   └── DailyLoadRow.cs
├── Pages/
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   ├── Result.cshtml
│   └── Result.cshtml.cs
├── Services/
│   ├── BubbleDetectionService.cs
│   ├── BubbleSheetProcessor.cs
│   ├── ExcelService.cs
│   ├── HandwritingService.cs
│   └── ImageProcessingService.cs
├── Templates/
│   └── Template1.cs
├── TestData/
│   ├── dotted_only.jpg
│   └── empty.jpg
├── wwwroot/
├── Program.cs
├── BubbleSheetReader.csproj
├── .gitignore
├── README.md
└── appsettings*.json
```

## Main code flow

### Bubble detection
The core detection logic lives in:

- Services/BubbleDetectionService.cs
- Templates/Template1.cs
- Models/BubbleSheetTemplate.cs

The detection service inspects each bubble region, calculates how dark it is, and compares the result to threshold values calibrated against sample sheets.

### Processing pipeline
The orchestration is handled in:

- Services/BubbleSheetProcessor.cs

This service:

- loads the image,
- identifies header selections,
- reads the stub handwriting values,
- analyzes daily rows,
- creates debug output,
- and returns the final result model.

### Output model
The result object is defined in:

- Models/BubbleSheetResult.cs

It includes important values such as:

- ID
- truck number
- shift
- crew
- daily rows
- warnings
- debug image URL

### Excel export
Workbook generation is handled in:

- Services/ExcelService.cs

This service writes a worksheet named Daily Load and populates the expected columns for downstream processing. It only writes rows that are considered valid/populated.

## Handwriting recognition

The current implementation intentionally includes placeholder handwriting values, not actual recognition logic. In the code, the service is named HandwritingStubService and returns obvious test data such as:

- NAME = TEST NAME
- DATE = 2026-09-22
- Start hours = 1000
- Finish hours = 1100
- Payload = 50
- Dump = TEST DUMP
- Time = 30

This is clearly marked in the app output as "fixed test values" and is not presented as real document extraction.

## Validation and sample sheets

The project includes sample images in the TestData folder for calibration and validation:

- dotted_only.jpg: a populated sheet used to test recognition behavior
- empty.jpg: a blank reference sheet used to test empty detection behavior

The project was validated against these sheets to confirm:

- ID detection can resolve a value such as 0264
- truck number detection can resolve a known number such as 12
- empty rows are recognized as empty when no valid selection exists
- ambiguous or multiple-selection rows are flagged for review rather than silently accepted

## Running the app

Requirements:

- .NET SDK 10
- a local machine with access to the repository

From the project root, run:

```bash
dotnet restore
dotnet build
dotnet run
```

Then open the local app URL shown in the terminal, typically:

```text
https://localhost:5001
```

or the ASP.NET generated localhost port in your environment.

## Upload workflow

1. Open the home page.
2. Upload a sheet image.
3. Submit the form.
4. Review the result page.
5. Inspect the debug image and warnings.
6. Download or process the generated Excel data as needed.

## Limitations

This repository is intentionally a focused MVP and not a general-purpose OCR pipeline. The key limitations are:

- it relies on fixed form geometry and calibrated coordinates
- it expects a consistent template size and layout
- it does not yet perform real handwriting or printed text recognition
- it does not support arbitrary scanned forms without template adjustment
- it is built for the specific bubble sheet test cases in the repository

## Future improvements

Possible follow-up work includes:

- real handwriting recognition using OCR or ML models
- broader support for additional sheet templates
- more robust detection thresholds for different scanners or lighting conditions
- PDF preprocessing and conversion workflow improvements
- better validation rules for ambiguous or partially filled forms

## Repository

Project repository:

https://github.com/Mohamed0khaled/BubbleSheetReader

## Summary

BubbleSheetReader is a compact, template-driven bubble-sheet reader built as a working MVP. It demonstrates how a fixed form can be recognized using geometry, darkness analysis, and simple output export without needing to build a full-scale OCR platform from day one.
