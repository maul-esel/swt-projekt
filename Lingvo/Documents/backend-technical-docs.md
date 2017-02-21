# Technical Documentation

## Table of Contents

## Introduction
The purpose of the application is to support students, especially refugees, in
learning German following the *Thannhauser Modell*. The *Thannhauser Modell* focuses on the ability to communicate in common situations over grammatical peculiarities. Students read and listen to German texts and repeat them. They can record their own voice, allowing them to compare their own pronounciation with that of a native speaker, and to track their progress. Since many refugees only receive lessons in German once a week, the ability to practice at home is thus essential. Our application facilitates this by providing a mobile app that can play and record such exercises.

## Structure (Static View)
The application follows the classical server-client architecture pattern: a server, or backend, manages a database containing a global library of workbooks and pages. Different clients read and manage this global library via a clearly defined HTTP interface.
First, an editor can access it via a web interface where he can create, edit, publish or delete workbooks. He can add pages to workbooks and create or upload recordings of these pages. Second, a mobile app is provided to refugees (or other students), allowing them to download these workbooks and pages, listen to them, and record their own voice.

The backend is realized as an ASP.NET Core application built on the .NET Core framework and runtime. It is thus cross-platform compatible, since it uses no platform-specific functionality. The mobile app is a Xamarin Forms app and is thus also based on the .NET Framework. This allows us to build both an Android and an iOS app with very little platform-specific code.

### Domain Model
An analysis of the domain resulted in the following model:
![](../Abgabe/DomainModel.png)
The concepts seen in the diagram are explained in the [glossary](../Abgabe/Glossar.pdf).

### Design Model
The design model shows how these concepts are mapped to design classes and how data storage and physical distribution are implemented.
![](../Abgabe/Designklassendiagramm.png)
The `Common` package contains those entities from the domain model present in both the app and the backend. These classes are implemented once and used by both projects.

By abstracting from concrete implementations of audio playback, recording and file storage, these are easily exchanged. In the case of audio playback and recording, this allows for the necessary platform-specific implementations. Additionally, the underlying file storage can easily be replaced, e.g. by implementations for the local file system or other providers such as Google.

### Backend Structure

The following UML component diagram illustrates the basic structure of the backend:

![](Images/Backend_Components.png)

It exposes two ports:

* a REST API that provides read-only JSON-formatted data to the mobile app
* and an Editing System that allows the user to manage the available workbooks and pages via a web interface.

These ports are implemented as a facade controller, split over multiple classes. Since the recommended architecture for ASP.NET Core is the Model-View-Controller pattern (MVC), this decision seems natural. The views are implemented in Razor, a scripting language that integrates C# with HTML. They are dynamically created on the server, hence very little client-side JavaScript is necessary. Specific view models exist for pages accepting user input. For display-only pages, the entity classes defined in the `Common` packages serve as models.

#### Internal Structure

A MySQL 5.7 database stores the workbooks and their pages as well as user information. Entity Framework Core is used as object-relational mapper, encapsulated in the `DatabaseService`. Recorded pages, i.e., MP3 files, are stored not in the database but in Azure Blob Storage. As discussed above, the `IStorage` interface abstracts from the concrete implementation. The `CloudLibrary` provides uniform handling of workbooks and pages across both data storages.

User management for the Editing System is implemented using the *ASP.NET Core Identity* framework. The relevant information is stored in the MySQL database as well.

As mentioned above, the conceptual facade controller is partitioned into 5 classes: `AppController` for the REST API, and `AccountController`, `WorkbookController`, `PageController` and `HomeController` for the Editing System. This separation has several reasons:

* Access Control: The need for authorization is specified as a class attribute. This makes it difficult to have methods for both the app and the Editing System in one class.
* Cohesion: Facade controllers typically have very low cohesion. For instance, fields relating to user management would be used by only three methods.
* Maintainability: By grouping the operations logically and reducing class size, maintainablity is improved.

### App Structure

Like the backend, the app also follows the MVC pattern. The most common pattern for Xamarin apps is ModelView-ViewModel (MVVM). However, MVC was chosen for two reasons:

* Use Case Controllers were already modeled as part of the business logic. Hence usage of MVVM would not have spared us the implementation of controllers.
* There are no view-specific states or properties in our app that are not already included in the model.

Almost all features are implemented in a shared project for both platforms. The only platform-specific classes are the audio player, the recorder, two custom views and WLAN-functionality.

## Dynamic View

Use Cases
SSDs
Kontrakte

## TOC
* Gesamtüberblick
  * Frameworks, platforms, libraries
  * App & backend
* static
  * Dömanenmodell
  * Designklassendiagramm
  * Komponentendiagramm
  * (Implementierungsklassendiagramm)
  * opt: Verteilung
* dynamic
  * Use Cases
  * SSDs
  * Kontrakte
* misc
  * Mocks
  * Glossar
  * projektplan, reviews
* non-functional
  * security: backend
  * usability, simplicity
  * reliablity

dazu jew begründung designentscheidung, weitere (nicht-Diagramm)-Fakten

# Technical Documentation: Backend

## Introduction
