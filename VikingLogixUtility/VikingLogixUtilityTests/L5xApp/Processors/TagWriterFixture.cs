using L5Sharp.Core;
using System.Collections.ObjectModel;
using VikingLogixUtility.Common;
using VikingLogixUtility.L5xApp.Interfaces;
using VikingLogixUtility.L5xApp.Models;
using VikingLogixUtility.L5xApp.Processors;
using VikingLogixUtility.L5xApp.ViewModels;
using VikingLogixUtilityTests.Extensions;

namespace VikingLogixUtilityTests.L5xApp.Processors
{
    [TestFixture]
    internal class TagWriterFixture
    {
        [Test]
        public void TestTagWriter()
        {
            var vm = new FakeViewModel();
            var content = L5X.Load("../../../L5xApp/Resources/Test.L5X");
            var et = ExportTable.Create();

            TagWriter.Geaux(vm, et, Helper.GetMappingRungs(content), content.Controller.Name, content.Tags);

            Assert.Multiple(() =>
            {
                Assert.That(et.Rows, Has.Count.EqualTo(7857));

                Assert.That(et.Rows[134][1], Is.EqualTo("AIT_205.EnableIn"));
                Assert.That(et.Rows[134][2], Is.EqualTo("Base"));
                Assert.That(et.Rows[134][3], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[134][4], Is.EqualTo("AnalogProcessing"));
                Assert.That(et.Rows[134][5], Is.EqualTo("BOOL"));
                Assert.That(et.Rows[134][6], Is.EqualTo("Oxidation Ditch No. 1\n"));
                Assert.That(et.Rows[134][7], Is.EqualTo("1"));
                Assert.That(et.Rows[134][8], Is.EqualTo("Local:11:I.Ch00.Data"));

                Assert.That(et.Rows[248]["Name"], Is.EqualTo("CP2Consumed.[0]"));
                Assert.That(et.Rows[248]["Description"], Is.EqualTo(string.Empty));

                Assert.That(et.Rows[249]["Name"], Is.EqualTo("CP2Consumed.[0].0"));
                Assert.That(et.Rows[249]["TagType"], Is.EqualTo("Consumed"));
                Assert.That(et.Rows[249]["DataType"], Is.EqualTo("DINT"));
                Assert.That(et.Rows[249]["MemberDataType"], Is.EqualTo("DINT"));
                Assert.That(et.Rows[249]["Description"], Is.EqualTo("RDS-1 Run Command"));
                Assert.That(et.Rows[249]["Value"], Is.EqualTo("0"));
                Assert.That(et.Rows[249]["Address"], Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void TestTagWriterExcludeMemberTags()
        {
            var vm = new FakeViewModel()
            {
                ExcludeMemberTags = true
            };
            var et = ExportTable.Create();
            var content = L5X.Load("../../../L5xApp/Resources/Test.L5X");

            TagWriter.Geaux(vm, et, Helper.GetMappingRungs(content), content.Controller.Name, content.Tags);

            Assert.Multiple(() =>
            {
                Assert.That(et.Rows, Has.Count.EqualTo(455));

                Assert.That(et.Rows[120]["Name"], Is.EqualTo("LSL_249B"));
                Assert.That(et.Rows[120]["TagType"], Is.EqualTo("Base"));
                Assert.That(et.Rows[120]["DataType"], Is.EqualTo("BOOL"));
                Assert.That(et.Rows[120]["MemberDataType"], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[120]["Description"], Is.EqualTo("Effluent Water System Water Low\n"));
                Assert.That(et.Rows[120]["Value"], Is.EqualTo("0"));
                Assert.That(et.Rows[120]["Address"], Is.EqualTo("Local:6:I.Pt06.Data"));
            });
        }

        [Test]
        public void TestAliasAddresses()
        {
            var content = L5X.Load("../../../L5xApp/Resources/AliasTest.L5X");
            var vm = new FakeViewModel();
            var et = ExportTable.Create();

            TagWriter.Geaux(vm, et, Helper.GetMappingRungs(content), content.Controller.Name, content.Tags);

            Assert.Multiple(() =>
            {
                Assert.That(et.Rows, Has.Count.EqualTo(4));

                Assert.That(et.Rows[0]["Scope"], Is.EqualTo("Testing"));
                Assert.That(et.Rows[0]["Name"], Is.EqualTo("AliasedAddressInput"));
                Assert.That(et.Rows[0]["TagType"], Is.EqualTo("Alias"));
                Assert.That(et.Rows[0]["AliasFor"], Is.EqualTo("[I/O]"));
                Assert.That(et.Rows[0]["DataType"], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[0]["MemberDataType"], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[0]["Description"], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[0]["Value"], Is.EqualTo(string.Empty));
                Assert.That(et.Rows[0]["Address"], Is.EqualTo("Local:1:I.Pt01.Data"));
            });
        }

        [Test]
        public void TestComments()
        {
            var content = L5X.Load("../../../L5xApp/Resources/CommentsTest.L5X");
            var vm = new FakeViewModel();
            var et = ExportTable.Create();

            TagWriter.Geaux(vm, et, Helper.GetMappingRungs(content), content.Controller.Name, content.Tags);

            Assert.Multiple(() =>
            {
                Assert.That(et.Rows, Has.Count.EqualTo(32));

                Assert.That(et.Rows[0].AsString(), Is.EqualTo("Testing,OneShot.[0],Base,,BOOL,BOOL,,0,"));
                Assert.That(et.Rows[1].AsString(), Is.EqualTo("Testing,OneShot.[1],Base,,BOOL,BOOL,Comment1,0,"));
                Assert.That(et.Rows[3].AsString(), Is.EqualTo("Testing,OneShot.[3],Base,,BOOL,BOOL,Comment3,0,"));
            });
        }

        [Test]
        public void TestComments2()
        {
            var content = L5X.Load("../../../L5xApp/Resources/CommentsTest2.L5X");
            var vm = new FakeViewModel();
            var et = ExportTable.Create();

            TagWriter.Geaux(vm, et, Helper.GetMappingRungs(content), content.Controller.Name, content.Tags);

            Assert.Multiple(() =>
            {
                Assert.That(et.Rows, Has.Count.EqualTo(34));

                Assert.That(et.Rows[0].AsString(), Is.EqualTo("Testing,OneShot.[0],Base,,INT,INT,,0,"));
                Assert.That(et.Rows[1].AsString(), Is.EqualTo("Testing,OneShot.[1],Base,,INT,INT,Comment1,0,"));
                Assert.That(et.Rows[2].AsString(), Is.EqualTo("Testing,OneShot.[2],Base,,INT,INT,,0,"));
                Assert.That(et.Rows[3].AsString(), Is.EqualTo("Testing,OneShot.[3],Base,,INT,INT,Comment3,0,"));
                Assert.That(et.Rows[4].AsString(), Is.EqualTo("Testing,OneShot.[3].1,Base,,INT,INT,Comment3.1,0,"));
                Assert.That(et.Rows[5].AsString(), Is.EqualTo("Testing,OneShot.[4],Base,,INT,INT,,0,"));
                Assert.That(et.Rows[6].AsString(), Is.EqualTo("Testing,OneShot.[5],Base,,INT,INT,,0,"));
                Assert.That(et.Rows[7].AsString(), Is.EqualTo("Testing,OneShot.[5].1,Base,,INT,INT,Comment5.1,0,"));
            });
        }

        private class FakeViewModel : ITagExportViewModel
        {
            public List<string> LogMessages { get; } = [];

            public bool IsRunning
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public bool ExcludeMemberTags { get; set; }

            public string L5XFilename
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public string L5XFilenameToolTip => throw new NotImplementedException();

            public ObservableCollection<ScopeItemViewModel> ScopeListBox
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public void Log(string message)
            {
                LogMessages.Add(message);
            }
        }
    }
}
