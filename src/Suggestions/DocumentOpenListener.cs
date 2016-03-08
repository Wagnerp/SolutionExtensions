﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using EnvDTE;

namespace SolutionExtensions
{
    class DocumentOpenListener
    {
        private DocumentEvents _events;

        private DocumentOpenListener(DocumentEvents events)
        {
            _events = events;
            _events.DocumentOpened += DocumentOpened;
        }

        public static DocumentOpenListener Instance { get; private set; }

        public static void Initialize(DocumentEvents events)
        {
            Instance = new DocumentOpenListener(events);
        }

        private void DocumentOpened(Document document)
        {
            if (!Path.IsPathRooted(document.FullName))
                return;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
           {
               IEnumerable<string> fileTypes;
               var result = SuggestionHandler.Instance.GetSuggestions(document.Name, out fileTypes);
               result.Extensions = result.Extensions.Where(e => e.Category != SuggestionFileModel.GENERAL);
               var missing = SuggestionHandler.Instance.GetMissingExtensions(result.Extensions);

               if (missing.Any() && result.Extensions.Any())
                   InfoBarService.Instance.ShowInfoBar(result, document.Name);

           }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
