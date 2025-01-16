using L5Sharp.Core;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.L5xApp.Processors;

namespace VikingLogixUtilityTests.L5xApp.Processors
{
    internal sealed class TestViewModel : ILoggable
    {
        public void Log(string message)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    internal sealed class AddressParserFixture
    {
        private readonly TestViewModel _vm = new();
        private readonly List<string> _mapping =
        [
            "XIC(Local:1:I.Pt00.Data)OTE(PSH_201A);",
            "XIC(XY_201A)OTE(Local:8:O.Pt00.Data);",
            "AnalogProcessing(AIT_205,Local:11:I.Ch00.Data,Local:11:I.Ch00.Fault)AnalogProcessing(AIT_206,Local:11:I.Ch01.Data,Local:11:I.Ch01.Fault)AnalogProcessing(LIT_255,Local:11:I.Ch02.Data,Local:11:I.Ch02.Fault)AnalogProcessing(LIT_256,Local:11:I.Ch03.Data,Local:11:I.Ch03.Fault);",
            "MOVE(SY_232,Local:13:I.Ch00.Data)MOVE(SY_233,Local:13:I.Ch01.Data)MOVE(SY_237,Local:13:I.Ch02.Data)MOVE(AO_1_Spare[3],Local:13:I.Ch03.Data);"
        ];

        [Test]
        public void ReturnsAddressDigitalInput()
        {
            Tag tag = new()
            {
                Name = "PSH_201A"
            };

            var address = AddressParser.GetAddress(_vm, _mapping, tag);

            Assert.That(address, Is.EqualTo("Local:1:I.Pt00.Data"));
        }

        [Test]
        public void ReturnsAddressDigitalOutput()
        {
            Tag tag = new()
            {
                Name = "XY_201A"
            };

            var address = AddressParser.GetAddress(_vm, _mapping, tag);

            Assert.That(address, Is.EqualTo("Local:8:O.Pt00.Data"));
        }

        [Test]
        public void ReturnsAddressAnalogInput()
        {
            Tag tag = new()
            {
                Name = "AIT_206"
            };

            var address = AddressParser.GetAddress(_vm, _mapping, tag);

            Assert.That(address, Is.EqualTo("Local:11:I.Ch01.Data"));
        }

        [Test]
        public void ReturnsAddressAnalogOutput()
        {
            Tag tag = new()
            {
                Name = "SY_233"
            };

            var address = AddressParser.GetAddress(_vm, _mapping, tag);

            Assert.That(address, Is.EqualTo("Local:13:I.Ch01.Data"));
        }

        [TestCase("<![CDATA[[OTL(FIT6220.ScaleEn)OTL(FIT6220.OOSEn)[MOV(Local:10:I.Ch15.Data,FIT6220_IN) ,AI_Alm(FIT6220,FIT6220_IN,Globals) ];]]",
            "FIT6220_IN",
            "Local:10:I.Ch15.Data")]
        [TestCase("<![CDATA[[OTL(UA9000.OOSEn) OTL(UA9000.ShutdownEn) ,XIC(EN2T_PCS_R2:9:I.Data.2) OTE(UA9000_IN) ,DI_Alm(UA9000,UA9000_IN,Globals) ];]]>",
            "UA9000_IN",
            "EN2T_PCS_R2:9:I.Data.2")]
        [TestCase("[OTE(MOV6220.LogicCtl) ,OTL(MOV6220.Mode) OTL(MOV6220.ModePerm) ,Valve(MOV6220,MOV6220_OUT,ZSC6220_IN,ZSO6220_IN,0,Globals) ,XIC(MOV6220_OUT) OTE(EN2T_PCS_R2:14:O.Data.8) ,XIO(MOV6220_OUT) OTE(EN2T_PCS_R2:14:O.Data.7) ,XIC(EN2T_PCS_R2:8:I.Data.6) OTE(ZSC6220_IN) ,XIC(EN2T_PCS_R2:8:I.Data.7) OTE(ZSO6220_IN) ];",
            "MOV6220_OUT",
            "EN2T_PCS_R2:14:O.Data.8")]
        [TestCase("Analog_AOI(TT_1090,RIO_1200_AENTR:11:I.Ch0Data,RIO_1200_AENTR:11:I.Ch0Fault,Global,AnalogConfig[3])Analog_AOI(AIT_1091A,RIO_1200_AENTR:11:I.Ch1Data,RIO_1200_AENTR:11:I.Ch1Fault,Global,AnalogConfig[3])Analog_AOI(AIT_1092A,RIO_1200_AENTR:11:I.Ch2Data,RIO_1200_AENTR:11:I.Ch2Fault,Global,AnalogConfig[3])Analog_AOI(AIT_1093A,RIO_1200_AENTR:11:I.Ch3Data,RIO_1200_AENTR:11:I.Ch3Fault,Global,AnalogConfig[3]);",
            "AIT_1091A",
            "RIO_1200_AENTR:11:I.Ch1Data")]
        [TestCase("MOV(Local:4:I.Ch09Data,DPIT4XX71_IN);",
            "DPIT4XX71_IN",
            "Local:4:I.Ch09Data")]
        public void TestGetAddressImprovements(string input, string tagName, string expected)
        {
            Tag tag = new()
            {
                Name = tagName
            };

            List<string> mapping =
            [
                input
            ];

            var address = AddressParser.GetAddress(_vm, mapping, tag);

            Assert.That(address, Is.EqualTo(expected));
        }

        [TestCase("XIC(Local:1:I.Data.0)OTE(ES4XX00_IN);", "ES4XX00", "")]
        public void TestNoWarningGivenWhenExactMatchNotFound(string input, string tagName, string expected)
        {
            Tag tag = new()
            {
                Name = tagName
            };

            List<string> mapping =
            [
                input
            ];

            string address = "test";

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => address = AddressParser.GetAddress(_vm, mapping, tag));
                Assert.That(address, Is.EqualTo(expected));
            });
        }

        [TestCase("[MOV(RACK_41:4:I.Ch10.Data,AIT20100A_IN),OTU(AIT20100A.LLEnable)OTU(AIT20100A.LEnable)OTL(AIT20100A.HEnable)OTL(AIT20100A.HHEnable),OTU(AIT20100A.LLBypEn)OTL(AIT20100A.HHBypEn)OTL(AIT20100A.OOSEn),OTL(AIT20100A.ScaleEn)AI_Alm(AIT20100A,AIT20100A_IN,Globals)]", 
            "AIT20100A")]
        public void TestTagNameFoundButNoMappingWithIt(string input, string tagName)
        {
            Tag tag = new()
            {
                Name = tagName
            };

            List<string> mapping =
            [
                input
            ];

            string address = "test";

            Assert.Multiple(() =>
            {
                Assert.Throws<NotImplementedException>(() => address = AddressParser.GetAddress(_vm, mapping, tag));
                Assert.That(address, Is.EqualTo("test"));
            });
        }
    }
}
