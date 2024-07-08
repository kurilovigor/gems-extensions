using Gems.Data;
using Gems.Data.UnitOfWork;
using Moq;

namespace Gems.Extensions.Test
{
    public class DynamicProxyTests
    {
        [Test]
        public async Task CallStoredProcedure_IntEnumerable()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallStoredProcedureAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(new List<int> { 123, 456 } as IEnumerable<int>));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var result = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .ProcIntEnumerableAsync("Val0", 567, cancelationTokenSource.Token);

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallStoredProcedureAsync<int>(
                "samples.proc_ints",
                It.Is<Dictionary<string, object>>(request =>
                    request.Count == 2 &&
                    (string)request["arg0"] == "Val0" &&
                    (int)request["arg1"] == 567),
                null));
            Assert.That(result.First(), Is.EqualTo(123));
            Assert.That(result.Skip(1).First(), Is.EqualTo(456));
        }

        [Test]
        public async Task CallStoredProcedureFirstOrDefault_Int()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallStoredProcedureFirstOrDefaultAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(123456));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var result = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .ProcCountAsync("Val0", 567, cancelationTokenSource.Token);

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallStoredProcedureFirstOrDefaultAsync<int>(
                "samples.proc_count",
                It.Is<Dictionary<string, object>>(request =>
                    request.Count == 2 &&
                    (string)request["arg_one"] == "Val0" &&
                    (int)request["arg_two"] == 567),
                null));
            Assert.That(result, Is.EqualTo(123456));
        }

        [Test]
        public async Task CallStoredProcedureFirstOrDefault_Dictionary()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallStoredProcedureFirstOrDefaultAsync<Dictionary<string, object>>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(new Dictionary<string, object> { ["a"] = "123abc" }));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var result = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .GetSettingsAsync("Val0", 567, cancelationTokenSource.Token);

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallStoredProcedureFirstOrDefaultAsync<Dictionary<string, object>>(
                "samples.get_settings",
                It.Is<Dictionary<string, object>>(request =>
                    request.Count == 2 &&
                    (string)request["arg0"] == "Val0" &&
                    (int)request["arg1"] == 567),
                null));
            Assert.That(result["a"], Is.EqualTo("123abc"));
        }

        [Test]
        public async Task CallStoredProcedure_Void()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallStoredProcedureAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.CompletedTask);
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            await uowpMock.Object
                .Repository<ISampleRepository1>()
                .DoWorkAsync("Val0", 567, SampleMetric.DoWork, 95, cancelationTokenSource.Token);

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallStoredProcedureAsync(
                "samples.do_work", 
                95,
                It.Is<Dictionary<string, object>>(request => 
                    request.Count == 2 &&
                    (string)request["arg0"] == "Val0" &&
                    (int)request["arg1"] == 567),
                SampleMetric.DoWork));
        }

        [Test]
        public async Task CallTableFunctionAsync()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallTableFunctionAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(new List<int>() { 123, 456 }));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var result = await uowpMock.Object
                .Repository<ISampleRepository1>("SampleUnitOfWork", cancelationTokenSource.Token)
                .GetItemsAsync();

            Assert.That(result[0], Is.EqualTo(123));
            Assert.That(result[1], Is.EqualTo(456));
            uowpMock.Verify(x => x.GetUnitOfWork("SampleUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallTableFunctionAsync<int>("samples.get_items", It.Is<Dictionary<string, object>>(request => request.Count == 0), null));
        }


        [Test]
        public async Task CallScalarFunction_Void()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallScalarFunctionAsync<object>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult((object)1));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            await uowpMock.Object
                .Repository<ISampleRepository1>()
                .DoWork2Async("123", 456, cancelationTokenSource.Token);

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallScalarFunctionAsync<object>(
                "samples.do_work2", 
                It.Is<Dictionary<string, object>>(request => 
                    request.Count == 2 &&
                    (string)request["p_arg0"] == "123" &&
                    (int)request["p_arg1"] == 456), null));
        }

        [Test]
        public async Task CallTableFunctionFirstAsync()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallTableFunctionFirstAsync<object>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult((object)1));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var result = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .GetObjectAsync("123", 456, cancelationTokenSource.Token);

            Assert.That(result, Is.EqualTo(1));

            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallTableFunctionFirstAsync<object>("samples.get_object", It.Is<Dictionary<string, object>>(request => request.Count == 2), null));
        }

        [Test]
        public async Task CallScalarFunction()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallScalarFunctionAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(123456));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var countResult = await uowpMock.Object
                .Repository<ISampleRepository1>("SampleUnitOfWork", cancelationTokenSource.Token)
                .ItemCountAsync();

            Assert.That(countResult, Is.EqualTo(123456));
            uowpMock.Verify(x => x.GetUnitOfWork("SampleUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallScalarFunctionAsync<int>("samples.item_count", It.Is<Dictionary<string, object>>(request => request.Count == 0), null));
        }

        [Test]
        public async Task CallScalarFunction_ImplicitRepositoryArguments()
        {
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallScalarFunctionAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(123456));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var countResult = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .ItemCountAsync();

            Assert.That(countResult, Is.EqualTo(123456));
            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", CancellationToken.None));
            uowMock.Verify(x => x.CallScalarFunctionAsync<int>("samples.item_count", It.Is<Dictionary<string, object>>(request => request.Count == 0), null));
        }

        [Test]
        public async Task CallScalarFunction_ImplicitRepositoryArguments_CancelationToken()
        {
            using var cancelationTokenSource = new CancellationTokenSource();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(x => x.CallScalarFunctionAsync<int>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<Enum>()))
                .Returns(Task.FromResult(123456));
            var uowpMock = new Mock<IUnitOfWorkProvider>();
            uowpMock
                .Setup(x => x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(uowMock.Object);

            var countResult = await uowpMock.Object
                .Repository<ISampleRepository1>()
                .ItemCountAsync(cancelationTokenSource.Token);

            Assert.That(countResult, Is.EqualTo(123456));
            uowpMock.Verify(x => x.GetUnitOfWork("DefaultUnitOfWork", cancelationTokenSource.Token));
            uowMock.Verify(x => x.CallScalarFunctionAsync<int>("samples.item_count", It.Is<Dictionary<string, object>>(request => request.Count == 0), null));
        }
    }
}
