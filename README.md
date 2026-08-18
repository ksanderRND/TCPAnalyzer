# TCPAnalyzer

A WPF desktop tool for analysing TCP (Tool Center Point) positioning measurements. Loads a comma-separated file of deviation values, computes statistics, highlights outliers, and plots results over repeated runs.

## Features

- **Statistics panel** — mean, min, max, and range of the Positional Error; mean and standard deviation per axis (X, Y, Z).
- **Outlier detection** — configurable threshold: 1σ, 2σ, 3σ above the mean PE, or a custom value entered manually. Outlier rows are highlighted in the measurement table.
- **Measurement table** — sortable grid showing each repetition's X, Y, Z, PE, and outlier flag.
- **Chart** — toggleable line plot of X, Y, Z, and PE over repetition number, displayed alongside the measurement table.
- **Partial-file resilience** — unparseable lines are skipped and reported; valid measurements are still loaded.

## Input file format

Plain text or CSV file, one measurement per line:

```
x,y,z
```

- `x`, `y`, `z` are the **deviations of the measured TCP position from the programmed target point**, not absolute machine coordinates. The target point is implicitly `(0, 0, 0)`.
- Values must use `.` as the decimal separator (invariant culture).
- Blank lines are ignored.
- Lines that cannot be parsed as three numbers are skipped; their line numbers are shown in the status bar.

Supported file extensions: `.txt`, `.csv`.

## What this tool measures: Accuracy, not Repeatability

This is an intentional simplification for v1.0 — it is important to understand when interpreting results.

The ISO 9283 standard for robot performance defines two distinct metrics:

- **Accuracy** — how close the measured position is to the **commanded (target) point**:
  `error = sqrt((X − Tx)² + (Y − Ty)² + (Z − Tz)²)`
- **Repeatability** — how tightly the measured positions cluster around their **own mean (barycentre)**, regardless of how far that mean is from the target:
  `error = sqrt((X − meanX)² + (Y − meanY)² + (Z − meanZ)²)`

These tell different stories. A machine can consistently return to the same point (good repeatability) while still being offset from the target (poor accuracy) — typically a sign that TCP calibration is needed, not mechanical repair. The reverse — large scatter around a mean close to zero — indicates poor repeatability with acceptable average accuracy.

**TCPAnalyzer v1.0 measures Accuracy only.** Because the input file already contains pre-computed deviations from the target, the target is implicitly `(0, 0, 0)` and `PositionalError = sqrt(X² + Y² + Z²)` gives the correct Accuracy value without needing to store or supply a separate target point. This keeps the file format simple (three numbers per line) while fully covering Accuracy analysis (`MeanPE`, `StdDevPE`, `MaxPE`, `MinPE`).

Repeatability is **not computed** in this version.

## Architecture

Built with **.NET 8**, **WPF**, and **OxyPlot 2.2** for charting, following the **MVVM** pattern.

- **Model** — immutable data classes (`Measurement`, `MeasurementStats`)
- **Service** — stateless logic (`MeasurementFileParser` via `IFileParser`, `StatisticsService`)
- **ViewModel** — presentation state with `INotifyPropertyChanged` and `INotifyDataErrorInfo`
- **Adapters** — `IFileDialog` abstracts the WPF file dialog from the ViewModel
- **View** — XAML layout with data bindings (`MainView`); converters for visibility logic

## Roadmap

- Unit tests;
- Async file loading for large datasets;
- File logging;
- Accept absolute TCP coordinates with explicit target point;
- Сompute Accuracy and Repeatability as independent metrics per ISO 9283;

