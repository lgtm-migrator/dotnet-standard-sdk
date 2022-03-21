﻿/**
* (C) Copyright IBM Corp. 2017, 2022.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.IO;
using IBM.Cloud.SDK.Core.Http;
using IBM.Cloud.SDK.Core.Http.Exceptions;
using IBM.Watson.Discovery.v1.Model;
using Environment = IBM.Watson.Discovery.v1.Model.Environment;
using IBM.Cloud.SDK.Core.Authentication.BasicAuth;

namespace IBM.Watson.Discovery.v1.UnitTests
{
    [TestClass]
    public class DiscoveryUnitTests
    {
        #region Constructor
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HttpClient_Null()
        {
            DiscoveryService service =
                new DiscoveryService(httpClient: null);
        }

        [TestMethod]
        public void Constructor_HttpClient()
        {
            DiscoveryService service =
                new DiscoveryService(CreateClient());

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Constructor()
        {
            DiscoveryService service =
                new DiscoveryService(new IBMHttpClient());

            Assert.IsNotNull(service);
        }
        #endregion

        #region Create Client
        private IClient CreateClient()
        {
            IClient client = Substitute.For<IClient>();

            client.WithAuthentication(Arg.Any<string>(), Arg.Any<string>())
                .Returns(client);

            return client;
        }
        #endregion

        #region Environments
        #region List Environments
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListEnvironments_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListEnvironments();
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListEnvironments_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListEnvironments();
        }

        [TestMethod]
        public void ListEnvironments_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response


            var diskUsage = Substitute.For<DiskUsage>();
            diskUsage.UsedBytes.Returns(0);
            diskUsage.MaximumAllowedBytes.Returns(0);

            DetailedResponse<ListEnvironmentsResponse> response = new DetailedResponse<ListEnvironmentsResponse>()
            {
                Result = new ListEnvironmentsResponse()
                {
                    Environments = new List<Environment>()
                    {
                       new Environment()
                       {
                           Status = Environment.StatusEnumValue.PENDING,
                           Name = "name",
                           Description = "description",
                           Size = Environment.SizeEnumValue.XS
                       }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<ListEnvironmentsResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListEnvironments();

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Environments != null);
            Assert.IsTrue(result.Result.Environments.Count > 0);
            Assert.IsTrue(result.Result.Environments[0].Status == Environment.StatusEnumValue.PENDING);
            Assert.IsTrue(result.Result.Environments[0].Name == "name");
            Assert.IsTrue(result.Result.Environments[0].Description == "description");
            Assert.IsTrue(result.Result.Environments[0].Size == Environment.SizeEnumValue.XS);
        }
        #endregion

        #region Create Environment
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateEnvironment_No_Environment()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateEnvironment(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateEnvironment_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.CreateEnvironment("environment");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void CreateEnvironment_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.CreateEnvironment("environment");
        }

        [TestMethod]
        public void CreateEnvironment_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<Environment> response = new DetailedResponse<Environment>()
            {
                Result = new Environment()
                {
                    Status = Environment.StatusEnumValue.PENDING,
                    Name = "name",
                    Description = "description",
                    Size = Environment.SizeEnumValue.XS,
                    IndexCapacity = new IndexCapacity()
                    {
                        DiskUsage = new DiskUsage() { }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("environment"))
                .Returns(request);
            request.As<Environment>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.CreateEnvironment("environment");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.Size == Environment.SizeEnumValue.XS);
        }
        #endregion

        #region Delete Environment
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteEnvironment_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteEnvironment(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteEnvironment_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteEnvironment("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteEnvironment_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteEnvironment("environmentId");
        }

        [TestMethod]
        public void DeleteEnvironment_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<DeleteEnvironmentResponse> response = new DetailedResponse<DeleteEnvironmentResponse>()
            {
                Result = new DeleteEnvironmentResponse()
                {
                    EnvironmentId = "environmentId",
                    Status = DeleteEnvironmentResponse.StatusEnumValue.DELETED
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DeleteEnvironmentResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteEnvironment("environmentId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.EnvironmentId == "environmentId");
            Assert.IsTrue(result.Result.Status == DeleteEnvironmentResponse.StatusEnumValue.DELETED);
        }
        #endregion

        #region Get Envronment
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetEnvironment_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetEnvironment(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetEnvironment_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetEnvironment("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetEnvironment_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetEnvironment("EnvironmentId");
        }

        [TestMethod]
        public void GetEnvironment_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<Environment> response = new DetailedResponse<Environment>()
            {
                Result = new Environment()
                {
                    Status = Environment.StatusEnumValue.PENDING,
                    Name = "name",
                    Description = "description",
                    Size = Environment.SizeEnumValue.XS
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<Environment>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetEnvironment("environmentId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == Environment.StatusEnumValue.PENDING);
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.Size == Environment.SizeEnumValue.XS);
        }
        #endregion

        #region Update Environment
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateEnvironment_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);

            service.UpdateEnvironment(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateEnvironment_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.UpdateEnvironment("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateEnvironment_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateEnvironment("environmentId");
        }

        [TestMethod]
        public void UpdateEnvironment_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<Environment> response = new DetailedResponse<Environment>()
            {
                Result = new Environment()
                {
                    Status = Environment.StatusEnumValue.PENDING,
                    Name = "name",
                    Description = "description",
                    Size = Environment.SizeEnumValue.XS
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("environment"))
                .Returns(request);
            request.As<Environment>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateEnvironment("environmentId");

            Assert.IsNotNull(result);
            client.Received().PutAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == Environment.StatusEnumValue.PENDING);
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.Size == Environment.SizeEnumValue.XS);
        }
        #endregion
        #endregion

        #region Configrations
        #region List Configurations
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListConfigurations_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListConfigurations(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListConfigurations_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListConfigurations("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListConfigurations_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListConfigurations("environmentId");
        }

        [TestMethod]
        public void ListConfigurations_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var configuration = Substitute.For<Configuration>();
            configuration.ConfigurationId.Returns("configurationId");
            configuration.Created.Returns(DateTime.MinValue);
            configuration.Updated.Returns(DateTime.MinValue);
            configuration.Name = "name";
            configuration.Description = "description";
            configuration.Conversions = new Conversions()
            {
                Pdf = new PdfSettings()
                {
                    Heading = new PdfHeadingDetection()
                    {
                        Fonts = new List<FontSetting>()
                        {
                            new FontSetting()
                            {
                                Level = 1,
                                Bold = false,
                                Italic = false,
                                Name = "name"
                            }
                        }
                    }
                },
                Word = new WordSettings()
                {
                    Heading = new WordHeadingDetection()
                    {
                        Fonts = new List<FontSetting>()
                        {
                            new FontSetting()
                            {
                                Level = 1,
                                Bold = false,
                                Italic = false,
                                Name = "name"
                            }
                        },
                        Styles = new List<WordStyle>()
                        {
                            new WordStyle()
                            {
                                Level = 1,
                                Names = new List<string>
                                {
                                    "name"
                                }
                            }
                        }
                    }
                },
                Html = new HtmlSettings()
                {
                    ExcludeTagsCompletely = new List<string>()
                    {
                        "exclude"
                    },
                    ExcludeTagsKeepContent = new List<string>()
                    {
                        "exclude but keep content"
                    },
                    KeepContent = new XPathPatterns()
                    {
                        Xpaths = new List<string>()
                        {
                            "keepContent"
                        }
                    },
                    ExcludeContent = new XPathPatterns()
                    {
                        Xpaths = new List<string>()
                        {
                            "excludeContent"
                        }
                    },
                    KeepTagAttributes = new List<string>()
                    {
                        "keepTagAttributes"
                    },
                    ExcludeTagAttributes = new List<string>()
                    {
                        "excludeTagAttributes"
                    }
                },
                JsonNormalizations = new List<NormalizationOperation>()
                {
                    new NormalizationOperation()
                    {

                    }
                },

            };
            configuration.Enrichments = new List<Enrichment>()
            {
                new Enrichment()
                {
                    Description = "description",
                    DestinationField = "destinationField",
                    SourceField = "sourceField",
                    Overwrite = false,
                    _Enrichment = "enrichmentName",
                    IgnoreDownstreamErrors = false,
                    Options = new EnrichmentOptions()
                    {
                        Features = new NluEnrichmentFeatures()
                        {
                        }
                    }
                }
            };
            configuration.Normalizations = new List<NormalizationOperation>()
            {
                new NormalizationOperation()
                {
                    Operation = NormalizationOperation.OperationEnumValue.MERGE,
                    SourceField = "sourceField",
                    DestinationField = "destinationField"
                }
            };

            DetailedResponse<ListConfigurationsResponse> response = Substitute.For<DetailedResponse<ListConfigurationsResponse>>();
            response.Result = Substitute.For<ListConfigurationsResponse>();
            response.Result.Configurations = Substitute.For<List<Configuration>>();
            response.Result.Configurations.Add(configuration);
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<ListConfigurationsResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListConfigurations("environmentId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsNotNull(result.Result.Configurations);
            Assert.IsTrue(result.Result.Configurations.Count > 0);
            Assert.IsTrue(result.Result.Configurations[0].Name == "name");
            Assert.IsTrue(result.Result.Configurations[0].Description == "description");
            Assert.IsNotNull(result.Result.Configurations[0].ConfigurationId);
            Assert.IsNotNull(result.Result.Configurations[0].Created);
            Assert.IsNotNull(result.Result.Configurations[0].Updated);
            Assert.IsNotNull(result.Result.Configurations[0].Enrichments);
            Assert.IsTrue(result.Result.Configurations[0].Enrichments.Count > 0);
            Assert.IsTrue(result.Result.Configurations[0].Enrichments[0].Description == "description");
            Assert.IsTrue(result.Result.Configurations[0].Enrichments[0].SourceField == "sourceField");
            Assert.IsTrue(result.Result.Configurations[0].Enrichments[0].Overwrite == false);
            Assert.IsTrue(result.Result.Configurations[0].Enrichments[0]._Enrichment == "enrichmentName");
            Assert.IsTrue(result.Result.Configurations[0].Enrichments[0].IgnoreDownstreamErrors == false);
            Assert.IsNotNull(result.Result.Configurations[0].Enrichments[0].Options);
        }
        #endregion

        #region Create Configuration
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateConfiguration_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateConfiguration(null, "configuration");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateConfiguration_No_Configuration()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateConfiguration("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateConfiguration_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.CreateConfiguration("environmentId", "configuration");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void CreateConfiguration_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.CreateConfiguration("environmentId", "configuration");
        }

        [TestMethod]
        public void CreateConfiguration_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            DetailedResponse<Configuration> Configuration = new DetailedResponse<Configuration>()
            {
                Result = new Configuration()
                {
                    Name = "name",
                    Description = "description",
                    Conversions = new Conversions()
                    {
                        Pdf = new PdfSettings()
                        {
                            Heading = new PdfHeadingDetection()
                            {
                                Fonts = new List<FontSetting>()
                                   {
                                       new FontSetting()
                                       {
                                           Level = 1,
                                           Bold = false,
                                           Italic = false,
                                           Name = "name"
                                       }
                                   }
                            }
                        }
                    },
                    Enrichments = new List<Enrichment>()
                {
                    new Enrichment()
                    {
                        Description = "description",
                        DestinationField = "destinationField",
                        SourceField = "sourceField",
                        Overwrite = false,
                        _Enrichment = "enrichmentName",
                        IgnoreDownstreamErrors = false,
                        Options = new EnrichmentOptions()
                        {
                            Features = new NluEnrichmentFeatures() { }
                        }
                    }
                },
                    Normalizations = new List<NormalizationOperation>()
                {
                    new NormalizationOperation()
                    {
                        Operation = NormalizationOperation.OperationEnumValue.MERGE,
                        SourceField = "sourceField",
                        DestinationField = "destinationField"
                    }
                }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBody<Configuration>(Arg.Any<Configuration>())
                .Returns(request);
            request.As<Configuration>()
                .Returns(Task.FromResult(Configuration));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.CreateConfiguration("environmentId", "name", "description");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
        }
        #endregion

        #region Delete Configuration
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteConfiguration_No_environmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteConfiguration(null, "configurationId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteConfiguration_No_ConfigurationId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteConfiguration("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteConfiguration_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteConfiguration("environmentId", "ConfigurationId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteConfiguration_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteConfiguration("environmentId", "configurationId");
        }

        [TestMethod]
        public void DeleteConfiguration_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var notice = Substitute.For<Notice>();
            notice.NoticeId.Returns("noticeId");
            notice.Created.Returns(DateTime.MinValue);
            notice.DocumentId.Returns("documentId");
            notice.Step.Returns("step");
            notice.Description.Returns("description");
            notice.QueryId.Returns("queryId");
            notice.Severity = Notice.SeverityEnumValue.ERROR;

            DetailedResponse<DeleteConfigurationResponse> response = new DetailedResponse<DeleteConfigurationResponse>()
            {
                Result = new DeleteConfigurationResponse()
                {
                    ConfigurationId = "ConfigurationId",
                    Status = DeleteConfigurationResponse.StatusEnumValue.DELETED,
                    Notices = new List<Notice>()
                    {
                        notice
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DeleteConfigurationResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteConfiguration("environmentId", "ConfigurationId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.ConfigurationId == "ConfigurationId");
            Assert.IsTrue(result.Result.Status == DeleteConfigurationResponse.StatusEnumValue.DELETED);
            Assert.IsNotNull(result.Result.Notices);
            Assert.IsTrue(result.Result.Notices.Count > 0);
            Assert.IsTrue(result.Result.Notices[0].Severity == Notice.SeverityEnumValue.ERROR);
            Assert.IsNotNull(result.Result.Notices[0].NoticeId);
            Assert.IsNotNull(result.Result.Notices[0].Created);
            Assert.IsNotNull(result.Result.Notices[0].DocumentId);
            Assert.IsNotNull(result.Result.Notices[0].Step);
            Assert.IsNotNull(result.Result.Notices[0].Description);
        }
        #endregion

        #region Get Configuration
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetConfiguration_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetConfiguration(null, "configurationId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetConfiguration_No_ConfigurationId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetConfiguration("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetConfiguration_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetConfiguration("environmentId", "ConfigurationId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetConfiguration_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetConfiguration("environmentId", "ConfigurationId");
        }

        [TestMethod]
        public void GetConfiguration_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<Configuration> response = new DetailedResponse<Configuration>()
            {
                Result = new Configuration()
                {
                    Name = "name",
                    Description = "description",
                    Conversions = new Conversions()
                    {
                        Pdf = new PdfSettings()
                        {
                            Heading = new PdfHeadingDetection()
                            {
                                Fonts = new List<FontSetting>()
                                   {
                                       new FontSetting()
                                       {
                                           Level = 1,
                                           Bold = false,
                                           Italic = false,
                                           Name = "name"
                                       }
                                   }
                            }
                        }
                    },
                    Enrichments = new List<Enrichment>()
                    {
                        new Enrichment()
                        {
                            Description = "description",
                            DestinationField = "destinationField",
                            SourceField = "sourceField",
                            Overwrite = false,
                            _Enrichment = "enrichmentName",
                            IgnoreDownstreamErrors = false,
                            Options = new EnrichmentOptions()
                            {
                                Features = new NluEnrichmentFeatures() { }
                            }
                        }
                    },
                    Normalizations = new List<NormalizationOperation>()
                    {
                        new NormalizationOperation()
                        {
                            Operation = NormalizationOperation.OperationEnumValue.MERGE,
                            SourceField = "sourceField",
                            DestinationField = "destinationField"
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<Configuration>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetConfiguration("environmentId", "ConfigurationId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
        }
        #endregion

        #region Update Configuration
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateConfiguration_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateConfiguration(null, "configurationId", "configuration");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateConfiguration_No_ConfigurationId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateConfiguration("environmentId", null, "configuration");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateConfiguration_No_Configuration()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateConfiguration("environmentId", "configurationId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateConfiguration_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.UpdateConfiguration("environmentId", "ConfigurationId", "configuration");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateConfiguration_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateConfiguration("environmentId", "ConfigurationId", "configuration");
        }

        [TestMethod]
        public void UpdateConfiguration_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                .Returns(request);

            #region response
            DetailedResponse<Configuration> response = new DetailedResponse<Configuration>()
            {
                Result = new Configuration()
                {
                    Name = "name",
                    Description = "description",
                    Conversions = new Conversions()
                    {
                        Pdf = new PdfSettings()
                        {
                            Heading = new PdfHeadingDetection()
                            {
                                Fonts = new List<FontSetting>()
                                   {
                                       new FontSetting()
                                       {
                                           Level = 1,
                                           Bold = false,
                                           Italic = false,
                                           Name = "name"
                                       }
                                   }
                            }
                        }
                    },
                    Enrichments = new List<Enrichment>()
                    {
                    new Enrichment()
                        {
                            Description = "description",
                            DestinationField = "destinationField",
                            SourceField = "sourceField",
                            Overwrite = false,
                            _Enrichment = "enrichmentName",
                            IgnoreDownstreamErrors = false,
                            Options = new EnrichmentOptions()
                            {
                                Features = new NluEnrichmentFeatures() { }
                            }
                        }
                    },
                    Normalizations = new List<NormalizationOperation>()
                    {
                        new NormalizationOperation()
                        {
                            Operation = NormalizationOperation.OperationEnumValue.MERGE,
                            SourceField = "sourceField",
                            DestinationField = "destinationField"
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBody<Configuration>(Arg.Any<Configuration>())
                .Returns(request);
            request.As<Configuration>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateConfiguration("environmentId", "ConfigurationId", "configuration");

            Assert.IsNotNull(result);
            client.Received().PutAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
        }
        #endregion
        #endregion

        #region Collections
        #region List Collections
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCollections_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListCollections(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCollections_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListCollections("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListCollections_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListCollections("environmentId");
        }

        [TestMethod]
        public void ListCollections_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<ListCollectionsResponse> response = new DetailedResponse<ListCollectionsResponse>()
            {
                Result = new ListCollectionsResponse()
                {
                    Collections = new List<Collection>()
                    {
                       new Collection()
                       {
                           Status = Collection.StatusEnumValue.PENDING,
                           Name = "name",
                           Description = "description",
                           ConfigurationId = "configurationId",
                           Language = "language",
                           DocumentCounts = new DocumentCounts() {}
                       }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<ListCollectionsResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListCollections("environmentId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsNotNull(result.Result.Collections);
            Assert.IsTrue(result.Result.Collections.Count > 0);
            Assert.IsTrue(result.Result.Collections[0].Name == "name");
            Assert.IsTrue(result.Result.Collections[0].Description == "description");
            Assert.IsTrue(result.Result.Collections[0].ConfigurationId == "configurationId");
            Assert.IsTrue(result.Result.Collections[0].Language == "language");
        }
        #endregion

        #region Create Collection
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateCollection_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateCollection(null, "collection");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateCollection_No_Body()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateCollection("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateCollection_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.CreateCollection("environmentId", "collection");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void CreateCollection_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.CreateCollection("environmentId", "collection");
        }

        [TestMethod]
        public void CreateCollection_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            DetailedResponse<Collection> response = new DetailedResponse<Collection>()
            {
                Result = new Collection()
                {
                    Name = "name",
                    Description = "description",
                    Status = Collection.StatusEnumValue.PENDING,
                    ConfigurationId = "configurationId",
                    Language = "language",
                    DocumentCounts = new DocumentCounts() { }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("colletion"))
                .Returns(request);
            request.As<Collection>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.CreateCollection("environmentId", "collection");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.Status == Collection.StatusEnumValue.PENDING);
            Assert.IsTrue(result.Result.ConfigurationId == "configurationId");
            Assert.IsTrue(result.Result.Language == "language");
        }
        #endregion

        #region Delete Collection
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCollection_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteCollection(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCollection_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteCollection("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCollection_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteCollection("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteCollection_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteCollection("environmentId", "collectionId");
        }

        [TestMethod]
        public void DeleteCollection_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<DeleteCollectionResponse> response = new DetailedResponse<DeleteCollectionResponse>()
            {
                Result = new DeleteCollectionResponse()
                {
                    CollectionId = "collectionId",
                    Status = DeleteCollectionResponse.StatusEnumValue.DELETED
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DeleteCollectionResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteCollection("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.CollectionId == "collectionId");
            Assert.IsTrue(result.Result.Status == DeleteCollectionResponse.StatusEnumValue.DELETED);
        }
        #endregion

        #region Get Collection
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCollection_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetCollection(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCollection_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetCollection("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCollection_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetCollection("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetCollection_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetCollection("environmentId", "collectionId");
        }

        [TestMethod]
        public void GetCollection_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            DetailedResponse<Collection> response = new DetailedResponse<Collection>()
            {
                Result = new Collection()
                {
                    Status = Collection.StatusEnumValue.PENDING,
                    Name = "name",
                    Description = "description",
                    ConfigurationId = "configurationId",
                    Language = "language",
                    DocumentCounts = new DocumentCounts() { }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<Collection>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetCollection("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.ConfigurationId == "configurationId");
            Assert.IsTrue(result.Result.Language == "language");
        }
        #endregion

        #region Update Collection
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCollection_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateCollection(null, "collectionId", "collection");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCollection_No_collectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateCollection("environmentId", null, "collection");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCollection_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.UpdateCollection("environmentId", "collectionId", "collection");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateCollection_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateCollection("environmentId", "collectionId", "collection");
        }

        [TestMethod]
        public void UpdateCollection_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                .Returns(request);

            #region response
            var documentCounts = Substitute.For<DocumentCounts>();
            documentCounts.Available.Returns(0);
            documentCounts.Processing.Returns(0);
            documentCounts.Failed.Returns(0);

            var response = Substitute.For<DetailedResponse<Collection>>();
            response.Result = Substitute.For<Collection>();
            response.Result.Status = Collection.StatusEnumValue.PENDING;
            response.Result.Name = "name";
            response.Result.Description = "description";
            response.Result.ConfigurationId = "configurationId";
            response.Result.Language = "language";
            response.Result.DocumentCounts = documentCounts;
            response.Result.CollectionId.Returns("collectionId");
            response.Result.Created.Returns(DateTime.MinValue);
            response.Result.Updated.Returns(DateTime.MinValue);

            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("collection"))
                .Returns(request);
            request.As<Collection>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateCollection("environmentId", "collectionId", "collection");

            Assert.IsNotNull(result);
            client.Received().PutAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Name == "name");
            Assert.IsTrue(result.Result.Description == "description");
            Assert.IsTrue(result.Result.ConfigurationId == "configurationId");
            Assert.IsTrue(result.Result.Language == "language");
            Assert.IsTrue(result.Result.CollectionId == "collectionId");
            Assert.IsNotNull(result.Result.Created);
            Assert.IsNotNull(result.Result.Updated);
            Assert.IsNotNull(result.Result.DocumentCounts.Available);
            Assert.IsNotNull(result.Result.DocumentCounts.Processing);
            Assert.IsNotNull(result.Result.DocumentCounts.Failed);
        }
        #endregion

        #region List Collection Fields
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCollectionFields_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListCollectionFields(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCollectionFields_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListCollectionFields("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCollectionFields_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListCollectionFields("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListCollectionFields_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListCollectionFields("environmentId", "collectionId");
        }

        [TestMethod]
        public void ListCollectionFields_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var field = Substitute.For<Field>();
            field._Field.Returns("field");
            field.Type = Field.TypeEnumValue.STRING;

            DetailedResponse<ListCollectionFieldsResponse> response = new DetailedResponse<ListCollectionFieldsResponse>()
            {
                Result = new ListCollectionFieldsResponse()
                {
                    Fields = new List<Field>()
                    {
                        field
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<ListCollectionFieldsResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListCollectionFields("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsNotNull(result.Result.Fields);
            Assert.IsTrue(result.Result.Fields.Count > 0);
            Assert.IsTrue(result.Result.Fields[0].Type == Field.TypeEnumValue.STRING);
            Assert.IsTrue(result.Result.Fields[0]._Field == "field");
        }
        #endregion
        #endregion

        #region Documents
        #region Add Document
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddDocument_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.AddDocument(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddDocument_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.AddDocument("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddDocument_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.AddDocument("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void AddDocument_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.AddDocument("environmentId", "collectionId");
        }

        [TestMethod]
        public void AddDocument_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            var notice = Substitute.For<Notice>();
            notice.NoticeId.Returns("noticeId");
            notice.Created.Returns(DateTime.MinValue);
            notice.DocumentId.Returns("documentId");
            notice.Step.Returns("step");
            notice.Description.Returns("description");
            notice.QueryId.Returns("queryId");
            notice.Severity = Notice.SeverityEnumValue.ERROR;

            var documentAccepted = new DetailedResponse<DocumentAccepted>()
            {
                Result = new DocumentAccepted()
                {
                    Status = DocumentAccepted.StatusEnumValue.PROCESSING,
                    DocumentId = "documentId",
                    Notices = new List<Notice>()
                    {
                        notice
                    }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(Arg.Any<MultipartFormDataContent>())
                .Returns(request);
            request.As<DocumentAccepted>()
                .Returns(Task.FromResult(documentAccepted));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.AddDocument("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == DocumentAccepted.StatusEnumValue.PROCESSING);
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsNotNull(result.Result.Notices);
            Assert.IsTrue(result.Result.Notices.Count > 0);
            Assert.IsTrue(result.Result.Notices[0].Severity == Notice.SeverityEnumValue.ERROR);
            Assert.IsNotNull(result.Result.Notices[0].NoticeId);
            Assert.IsNotNull(result.Result.Notices[0].Created);
            Assert.IsNotNull(result.Result.Notices[0].DocumentId);
            Assert.IsNotNull(result.Result.Notices[0].Step);
            Assert.IsNotNull(result.Result.Notices[0].Description);
        }

        [TestMethod]
        public void AddDocument_Success_WithConfiguration()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            var notice = Substitute.For<Notice>();
            notice.NoticeId.Returns("noticeId");
            notice.Created.Returns(DateTime.MinValue);
            notice.DocumentId.Returns("documentId");
            notice.Step.Returns("step");
            notice.Description.Returns("description");
            notice.QueryId.Returns("queryId");
            notice.Severity = Notice.SeverityEnumValue.ERROR;

            var documentAccepted = new DetailedResponse<DocumentAccepted>()
            {
                Result = new DocumentAccepted()
                {
                    Status = DocumentAccepted.StatusEnumValue.PROCESSING,
                    DocumentId = "documentId",
                    Notices = new List<Notice>()
                    {
                        notice
                    }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(Arg.Any<MultipartFormDataContent>())
                .Returns(request);
            request.As<DocumentAccepted>()
                .Returns(Task.FromResult(documentAccepted));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.AddDocument("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == DocumentAccepted.StatusEnumValue.PROCESSING);
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsNotNull(result.Result.Notices);
            Assert.IsTrue(result.Result.Notices.Count > 0);
            Assert.IsTrue(result.Result.Notices[0].Severity == Notice.SeverityEnumValue.ERROR);
            Assert.IsNotNull(result.Result.Notices[0].NoticeId);
            Assert.IsNotNull(result.Result.Notices[0].Created);
            Assert.IsNotNull(result.Result.Notices[0].DocumentId);
            Assert.IsNotNull(result.Result.Notices[0].Step);
            Assert.IsNotNull(result.Result.Notices[0].Description);
        }
        #endregion

        #region Delete Document
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteDocument_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteDocument(null, "collectionId", "doucmentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteDocument_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteDocument("environmentId", null, "documentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteDocument_No_DocumentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteDocument("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteDocument_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteDocument("environmentId", "collectionId", "doucmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteDocument_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteDocument("environmentId", "collectionId", "doucmentId");
        }

        [TestMethod]
        public void DeleteDocument_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<DeleteDocumentResponse>()
            {
                Result = new DeleteDocumentResponse()
                {
                    DocumentId = "doucmentId",
                    Status = DeleteDocumentResponse.StatusEnumValue.DELETED
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DeleteDocumentResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteDocument("environmentId", "collectionId", "doucmentId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.DocumentId == "doucmentId");
            Assert.IsTrue(result.Result.Status == DeleteDocumentResponse.StatusEnumValue.DELETED);
        }
        #endregion

        #region Get Document
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetDocument_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetDocumentStatus(null, "collectionId", "documentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetDocument_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetDocumentStatus("environmentId", null, "documentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetDocument_No_DocumentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetDocumentStatus("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetDocument_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetDocumentStatus("environmentId", "collectionId", "documentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetDocument_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetDocumentStatus("environmentId", "collectionId", "documentId");
        }

        [TestMethod]
        public void GetDocument_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = Substitute.For<DetailedResponse<DocumentStatus>>();
            response.Result = Substitute.For<DocumentStatus>();
            response.Result.Status = DocumentStatus.StatusEnumValue.AVAILABLE;
            response.Result.FileType = DocumentStatus.FileTypeEnumValue.HTML;
            response.Result.Filename = "fileName";
            response.Result.Sha1 = "sha1";
            response.Result.DocumentId.Returns("documentId");
            response.Result.ConfigurationId.Returns("configurationId");
            response.Result.StatusDescription.Returns("statusDescription");
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DocumentStatus>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetDocumentStatus("environmentId", "collectionId", "documentId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == DocumentStatus.StatusEnumValue.AVAILABLE);
            Assert.IsNotNull(result.Result.DocumentId);
            Assert.IsNotNull(result.Result.ConfigurationId);
            Assert.IsNotNull(result.Result.StatusDescription);
        }
        #endregion

        #region Update Document
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateDocument_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateDocument(null, "collectionId", "documentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateDocument_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateDocument("environmentId", null, "documentId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateDocument_No_DocumentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateDocument("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateDocument_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.UpdateDocument("environmentId", "collectionId", "documentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateDocument_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateDocument("environmentId", "collectionId", "documentId");
        }

        [TestMethod]
        public void UpdateDocument_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            var notice = Substitute.For<Notice>();
            notice.NoticeId.Returns("noticeId");
            notice.Created.Returns(DateTime.MinValue);
            notice.DocumentId.Returns("documentId");
            notice.Step.Returns("step");
            notice.Description.Returns("description");
            notice.QueryId.Returns("queryId");
            notice.Severity = Notice.SeverityEnumValue.ERROR;

            var documentAccepted = new DetailedResponse<DocumentAccepted>()
            {
                Result = new DocumentAccepted()
                {
                    Status = DocumentAccepted.StatusEnumValue.PROCESSING,
                    DocumentId = "documentId",
                    Notices = new List<Notice>()
                    {
                        notice
                    }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(Arg.Any<MultipartFormDataContent>())
                .Returns(request);
            request.As<DocumentAccepted>()
                .Returns(Task.FromResult(documentAccepted));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateDocument("environmentId", "collectionId", "documentId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == DocumentAccepted.StatusEnumValue.PROCESSING);
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsNotNull(result.Result.Notices);
            Assert.IsTrue(result.Result.Notices.Count > 0);
            Assert.IsTrue(result.Result.Notices[0].Severity == Notice.SeverityEnumValue.ERROR);
            Assert.IsNotNull(result.Result.Notices[0].NoticeId);
            Assert.IsNotNull(result.Result.Notices[0].Created);
            Assert.IsNotNull(result.Result.Notices[0].DocumentId);
            Assert.IsNotNull(result.Result.Notices[0].Step);
            Assert.IsNotNull(result.Result.Notices[0].Description);
        }

        [TestMethod]
        public void UpdateDocument_Success_WithConfiguration()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            var notice = Substitute.For<Notice>();
            notice.NoticeId.Returns("noticeId");
            notice.Created.Returns(DateTime.MinValue);
            notice.DocumentId.Returns("documentId");
            notice.Step.Returns("step");
            notice.Description.Returns("description");
            notice.QueryId.Returns("queryId");
            notice.Severity = Notice.SeverityEnumValue.ERROR;

            var documentAccepted = new DetailedResponse<DocumentAccepted>()
            {
                Result = new DocumentAccepted()
                {
                    Status = DocumentAccepted.StatusEnumValue.PROCESSING,
                    DocumentId = "documentId",
                    Notices = new List<Notice>()
                    {
                        notice
                    }
                }
            };

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(Arg.Any<MultipartFormDataContent>())
                .Returns(request);
            request.As<DocumentAccepted>()
                .Returns(Task.FromResult(documentAccepted));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateDocument("environmentId", "collectionId", "documentId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.Status == DocumentAccepted.StatusEnumValue.PROCESSING);
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsNotNull(result.Result.Notices);
            Assert.IsTrue(result.Result.Notices.Count > 0);
            Assert.IsTrue(result.Result.Notices[0].Severity == Notice.SeverityEnumValue.ERROR);
            Assert.IsNotNull(result.Result.Notices[0].NoticeId);
            Assert.IsNotNull(result.Result.Notices[0].Created);
            Assert.IsNotNull(result.Result.Notices[0].DocumentId);
            Assert.IsNotNull(result.Result.Notices[0].Step);
            Assert.IsNotNull(result.Result.Notices[0].Description);
        }
        #endregion
        #endregion

        #region Query
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Query_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Query(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Query_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Query("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Query_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.Query("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void Query_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.Query("environmentId", "collectionId");
        }

        [TestMethod]
        public void Query_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<QueryResponse>()
            {
                Result = new QueryResponse()
                {
                    MatchingResults = 1,
                    Results = new List<QueryResult>()
                {
                    new QueryResult()
                    {
                        Id = "id",
                        Metadata = new Dictionary<string, object>()
                    }
                },
                    Aggregations = new List<QueryAggregation>()
                    {
                        new QueryAggregation()
                        {
                            Type = "type",
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("query"))
                .Returns(request);
            request.As<QueryResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.Query("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
        }
        #endregion

        #region Notices
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Notices_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.QueryNotices(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Notices_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.QueryNotices("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Notices_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.QueryNotices("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void Notices_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.QueryNotices("environmentId", "collectionId");
        }

        [TestMethod]
        public void Notices_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<QueryNoticesResponse>()
            {
                Result = new QueryNoticesResponse()
                {
                    MatchingResults = 1,
                    Results = new List<QueryNoticesResult>()
                {
                    new QueryNoticesResult()
                    {
                        Id = "id",
                        Metadata = new Dictionary<string, object>()
                    }
                },
                    Aggregations = new List<QueryAggregation>()
                    {
                        new QueryAggregation()
                        {
                            Type = "type",
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<QueryNoticesResponse>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.QueryNotices("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsNotNull(result.Result.Results);
            Assert.IsTrue(result.Result.Results.Count > 0);
            Assert.IsNotNull(result.Result.Results[0].Id);
            Assert.IsNotNull(result.Result.Results[0].Metadata);
        }
        #endregion

        #region Training Data
        #region Delete Training Data
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteAllTrainingData_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteAllTrainingData(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteAllTrainingData_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteAllTrainingData("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteAllTrainingData_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteAllTrainingData("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteAllTrainingData_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteAllTrainingData("environmentId", "collectionId");
        }

        [TestMethod]
        public void DeleteAllTrainingData_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<object>();
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<object>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteAllTrainingData("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
        }
        #endregion

        #region List Training Data
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListTrainingData_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListTrainingData(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListTrainingData_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListTrainingData("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListTrainingData_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListTrainingData("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListTrainingData_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListTrainingData("environmentId", "collectionId");
        }

        [TestMethod]
        public void ListTrainingData_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<TrainingDataSet>()
            {
                Result = new TrainingDataSet()
                {
                    EnvironmentId = "environmentId",
                    CollectionId = "collectionId",
                    Queries = new List<TrainingQuery>()
                    {
                        new TrainingQuery()
                        {
                            QueryId = "queryId",
                            NaturalLanguageQuery = "naturalLanguageQuery",
                            Filter = "filter",
                            Examples = new List<TrainingExample>()
                            {
                                new TrainingExample()
                                {
                                    DocumentId = "documentId",
                                    CrossReference = "crossReference",
                                    Relevance = 1
                                }
                            }
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<TrainingDataSet>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListTrainingData("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.EnvironmentId == "environmentId");
            Assert.IsTrue(result.Result.CollectionId == "collectionId");
            Assert.IsNotNull(result.Result.Queries);
            Assert.IsTrue(result.Result.Queries.Count > 0);
            Assert.IsTrue(result.Result.Queries[0].QueryId == "queryId");
            Assert.IsTrue(result.Result.Queries[0].NaturalLanguageQuery == "naturalLanguageQuery");
            Assert.IsTrue(result.Result.Queries[0].Filter == "filter");
            Assert.IsNotNull(result.Result.Queries[0].Examples);
            Assert.IsTrue(result.Result.Queries[0].Examples.Count > 0);
            Assert.IsTrue(result.Result.Queries[0].Examples[0].DocumentId == "documentId");
            Assert.IsTrue(result.Result.Queries[0].Examples[0].CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Queries[0].Examples[0].Relevance == 1.0f);
        }
        #endregion

        #region Add Training Data
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddTrainingData_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.AddTrainingData(null, "collectionId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddTrainingData_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.AddTrainingData("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddTrainingData_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.AddTrainingData("environmentId", "collectionId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void AddTrainingData_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.AddTrainingData("environmentId", "collectionId");
        }

        [TestMethod]
        public void AddTrainingData_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            #region response
            var response = new DetailedResponse<TrainingQuery>()
            {
                Result = new TrainingQuery()
                {
                    QueryId = "queryId",
                    NaturalLanguageQuery = "naturalLanguageQuery",
                    Filter = "filter",
                    Examples = new List<TrainingExample>()
                    {
                        new TrainingExample()
                        {
                            DocumentId = "documentId",
                            CrossReference = "crossReference",
                            Relevance = 1
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("query"))
                .Returns(request);
            request.As<TrainingQuery>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.AddTrainingData("environmentId", "collectionId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.QueryId == "queryId");
            Assert.IsTrue(result.Result.NaturalLanguageQuery == "naturalLanguageQuery");
            Assert.IsTrue(result.Result.Filter == "filter");
            Assert.IsNotNull(result.Result.Examples);
            Assert.IsTrue(result.Result.Examples.Count > 0);
            Assert.IsTrue(result.Result.Examples[0].DocumentId == "documentId");
            Assert.IsTrue(result.Result.Examples[0].CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Examples[0].Relevance == 1.0f);
        }
        #endregion

        #region Delete Query
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingData_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingData(null, "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingData_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingData("environmentId", null, "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingData_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingData("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingData_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteTrainingData("environmentId", "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteTrainingData_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteTrainingData("environmentId", "collectionId", "queryId");
        }

        [TestMethod]
        public void DeleteTrainingData_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<object>();
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<object>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteTrainingData("environmentId", "collectionId", "queryId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
        }
        #endregion

        #region Get Training Data
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingData_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingData(null, "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingData_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingData("environmentId", null, "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingData_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingData("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingData_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetTrainingData("environmentId", "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetTrainingData_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetTrainingData("environmentId", "collectionId", "queryId");
        }

        [TestMethod]
        public void GetTrainingData_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<TrainingQuery>()
            {
                Result = new TrainingQuery()
                {
                    QueryId = "queryId",
                    NaturalLanguageQuery = "naturalLanguageQuery",
                    Filter = "filter",
                    Examples = new List<TrainingExample>()
                    {
                        new TrainingExample()
                        {
                            DocumentId = "documentId",
                            CrossReference = "crossReference",
                            Relevance = 1
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<TrainingQuery>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetTrainingData("environmentId", "collectionId", "queryId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.QueryId == "queryId");
            Assert.IsTrue(result.Result.NaturalLanguageQuery == "naturalLanguageQuery");
            Assert.IsTrue(result.Result.Filter == "filter");
            Assert.IsNotNull(result.Result.Examples);
            Assert.IsTrue(result.Result.Examples.Count > 0);
            Assert.IsTrue(result.Result.Examples[0].DocumentId == "documentId");
            Assert.IsTrue(result.Result.Examples[0].CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Examples[0].Relevance == 1.0f);
        }
        #endregion

        #region Get Training Examples
        //  Not implemented.
        #endregion

        #region Add Example
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateTrainingExample_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateTrainingExample(null, "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateTrainingExample_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateTrainingExample("environmentId", null, "queryId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateTrainingExample_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateTrainingExample("environmentId", "collectionId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateTrainingExample_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.CreateTrainingExample("environmentId", "collectionId", "queryId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void CreateTrainingExample_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.CreateTrainingExample("environmentId", "collectionId", "queryId");
        }

        [TestMethod]
        public void CreateTrainingExample_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<TrainingExample>()
            {
                Result = new TrainingExample()
                {
                    DocumentId = "documentId",
                    CrossReference = "crossReference",
                    Relevance = 1
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBody(Arg.Any<TrainingExample>())
                .Returns(request);
            request.As<TrainingExample>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.CreateTrainingExample("environmentId", "collectionId", "queryId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsTrue(result.Result.CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Relevance == 1.0f);
        }
        #endregion

        #region Remove Example Document
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingExample_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingExample(null, "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingExample_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingExample("environmentId", null, "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingExample_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingExample("environmentId", "collectionId", null, "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingExample_No_ExampleId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteTrainingExample("environmentId", "collectionId", "queryId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteTrainingExample_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteTrainingExample_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod]
        public void DeleteTrainingExample_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<object>();
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<object>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteTrainingExample("environmentId", "collectionId", "queryId", "exampleId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
        }
        #endregion

        #region Get Example Details
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingExample_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingExample(null, "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingExample_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingExample("environmentId", null, "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingExample_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingExample("environmentId", "collectionId", null, "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingExample_No_ExampleId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetTrainingExample("environmentId", "collectionId", "queryId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetTrainingExample_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetTrainingExample_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod]
        public void GetTrainingExample_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<TrainingExample>()
            {
                Result = new TrainingExample()
                {
                    DocumentId = "documentId",
                    CrossReference = "crossReference",
                    Relevance = 1
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<TrainingExample>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetTrainingExample("environmentId", "collectionId", "queryId", "exampleId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsTrue(result.Result.CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Relevance == 1.0f);
        }
        #endregion

        #region Update Example
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTrainingExample_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateTrainingExample(null, "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTrainingExample_No_CollectionId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateTrainingExample("environmentId", null, "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTrainingExample_No_QueryId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateTrainingExample("environmentId", "collectionId", null, "exampleId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTrainingExample_No_ExampleId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateTrainingExample("environmentId", "collectionId", "queryId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTrainingExample_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.UpdateTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateTrainingExample_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateTrainingExample("environmentId", "collectionId", "queryId", "exampleId");
        }

        [TestMethod]
        public void UpdateTrainingExample_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<TrainingExample>()
            {
                Result = new TrainingExample()
                {
                    DocumentId = "documentId",
                    CrossReference = "crossReference",
                    Relevance = 1
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBodyContent(new StringContent("example"))
                .Returns(request);
            request.As<TrainingExample>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateTrainingExample("environmentId", "collectionId", "queryId", "exampleId");

            Assert.IsNotNull(result);
            client.Received().PutAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.DocumentId == "documentId");
            Assert.IsTrue(result.Result.CrossReference == "crossReference");
            Assert.IsTrue(result.Result.Relevance == 1.0f);
        }
        #endregion
        #endregion

        #region Credentials
        #region List Credentials
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCredentials_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.ListCredentials(null);
        }


        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ListCredentials_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.ListCredentials("environmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ListCredentials_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.ListCredentials("environmentId");
        }

        [TestMethod]
        public void ListCredentials_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<CredentialsList>()
            {
                Result = new CredentialsList()
                {
                    Credentials = new List<Credentials>()
                    {
                        new Credentials()
                        {
                            SourceType = Credentials.SourceTypeEnumValue.BOX,
                            CredentialDetails = new CredentialDetails()
                            {
                                ClientId = "clientId",
                                ClientSecret = "clientSecret",
                                CredentialType = CredentialDetails.CredentialTypeEnumValue.OAUTH2,
                                EnterpriseId = "enterpriseId",
                                OrganizationUrl = "organizationUrl",
                                Passphrase = "passphrase",
                                Password = "password",
                                PrivateKey = "privateKey",
                                PublicKeyId = "publicKeyId",
                                SiteCollectionPath = "siteCollectionPath",
                                Url = "url",
                                Username = "username"
                            }
                        }
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<CredentialsList>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.ListCredentials("environmentId");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsNotNull(result.Result.Credentials);
            Assert.IsNotNull(result.Result.Credentials[0]);
            Assert.IsTrue(result.Result.Credentials[0].SourceType == Credentials.SourceTypeEnumValue.BOX);
            Assert.IsNotNull(result.Result.Credentials[0].CredentialDetails);
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.ClientId == "clientId");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.ClientSecret == "clientSecret");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.CredentialType == CredentialDetails.CredentialTypeEnumValue.OAUTH2);
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.EnterpriseId == "enterpriseId");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.OrganizationUrl == "organizationUrl");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.Passphrase == "passphrase");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.Password == "password");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.PrivateKey == "privateKey");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.PublicKeyId == "publicKeyId");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.SiteCollectionPath == "siteCollectionPath");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.Url == "url");
            Assert.IsTrue(result.Result.Credentials[0].CredentialDetails.Username == "username");
        }
        #endregion

        #region Create Credentials
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateCredentials_No_Environment()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.CreateCredentials(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CreateCredentials_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;

            service.CreateCredentials("environtmentId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void CreateCredentials_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.CreateCredentials("environmentId");
        }

        [TestMethod]
        public void CreateCredentials_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PostAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<Credentials>()
            {
                Result = new Credentials()
                {
                    SourceType = Credentials.SourceTypeEnumValue.BOX,
                    CredentialDetails = new CredentialDetails()
                    {
                        ClientId = "clientId",
                        ClientSecret = "clientSecret",
                        CredentialType = CredentialDetails.CredentialTypeEnumValue.OAUTH2,
                        EnterpriseId = "enterpriseId",
                        OrganizationUrl = "organizationUrl",
                        Passphrase = "passphrase",
                        Password = "password",
                        PrivateKey = "privateKey",
                        PublicKeyId = "publicKeyId",
                        SiteCollectionPath = "siteCollectionPath",
                        Url = "url",
                        Username = "username"
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBody<Credentials>(new Credentials())
                .Returns(request);
            request.As<Credentials>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.CreateCredentials("environmentId");

            Assert.IsNotNull(result);
            client.Received().PostAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.SourceType == Credentials.SourceTypeEnumValue.BOX);
            Assert.IsNotNull(result.Result.CredentialDetails);
            Assert.IsTrue(result.Result.CredentialDetails.ClientId == "clientId");
            Assert.IsTrue(result.Result.CredentialDetails.ClientSecret == "clientSecret");
            Assert.IsTrue(result.Result.CredentialDetails.CredentialType == CredentialDetails.CredentialTypeEnumValue.OAUTH2);
            Assert.IsTrue(result.Result.CredentialDetails.EnterpriseId == "enterpriseId");
            Assert.IsTrue(result.Result.CredentialDetails.OrganizationUrl == "organizationUrl");
            Assert.IsTrue(result.Result.CredentialDetails.Passphrase == "passphrase");
            Assert.IsTrue(result.Result.CredentialDetails.Password == "password");
            Assert.IsTrue(result.Result.CredentialDetails.PrivateKey == "privateKey");
            Assert.IsTrue(result.Result.CredentialDetails.PublicKeyId == "publicKeyId");
            Assert.IsTrue(result.Result.CredentialDetails.SiteCollectionPath == "siteCollectionPath");
            Assert.IsTrue(result.Result.CredentialDetails.Url == "url");
            Assert.IsTrue(result.Result.CredentialDetails.Username == "username");
        }
        #endregion

        #region Get Credential
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCredentials_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetCredentials(null, "credentialId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCredentials_No_CredentialId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.GetCredentials("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetCredentials_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.GetCredentials("environmentId", "credentialId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void GetCredentials_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";
            service.GetCredentials("environmentId", "credentialId");
        }

        [TestMethod]
        public void GetCredentials_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.GetAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<Credentials>()
            {
                Result = new Credentials()
                {
                    SourceType = Credentials.SourceTypeEnumValue.BOX,
                    CredentialDetails = new CredentialDetails()
                    {
                        ClientId = "clientId",
                        ClientSecret = "clientSecret",
                        CredentialType = CredentialDetails.CredentialTypeEnumValue.OAUTH2,
                        EnterpriseId = "enterpriseId",
                        OrganizationUrl = "organizationUrl",
                        Passphrase = "passphrase",
                        Password = "password",
                        PrivateKey = "privateKey",
                        PublicKeyId = "publicKeyId",
                        SiteCollectionPath = "siteCollectionPath",
                        Url = "url",
                        Username = "username"
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<Credentials>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.GetCredentials("environmentId", "credentailID");

            Assert.IsNotNull(result);
            client.Received().GetAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.SourceType == Credentials.SourceTypeEnumValue.BOX);
            Assert.IsNotNull(result.Result.CredentialDetails);
            Assert.IsTrue(result.Result.CredentialDetails.ClientId == "clientId");
            Assert.IsTrue(result.Result.CredentialDetails.ClientSecret == "clientSecret");
            Assert.IsTrue(result.Result.CredentialDetails.CredentialType == CredentialDetails.CredentialTypeEnumValue.OAUTH2);
            Assert.IsTrue(result.Result.CredentialDetails.EnterpriseId == "enterpriseId");
            Assert.IsTrue(result.Result.CredentialDetails.OrganizationUrl == "organizationUrl");
            Assert.IsTrue(result.Result.CredentialDetails.Passphrase == "passphrase");
            Assert.IsTrue(result.Result.CredentialDetails.Password == "password");
            Assert.IsTrue(result.Result.CredentialDetails.PrivateKey == "privateKey");
            Assert.IsTrue(result.Result.CredentialDetails.PublicKeyId == "publicKeyId");
            Assert.IsTrue(result.Result.CredentialDetails.SiteCollectionPath == "siteCollectionPath");
            Assert.IsTrue(result.Result.CredentialDetails.Url == "url");
            Assert.IsTrue(result.Result.CredentialDetails.Username == "username");
        }
        #endregion

        #region Delete Credential
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCredentials_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteCredentials(null, "credentialId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCredentials_No_CredentialId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.DeleteCredentials("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCredentials_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.DeleteCredentials("environmentId", "credentialId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void DeleteCredentials_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.DeleteCredentials("environmentId", "credentialId");
        }

        [TestMethod]
        public void DeleteCredentials_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.DeleteAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<DeleteCredentials>()
            {
                Result = new DeleteCredentials()
                {
                    CredentialId = "credentialId",
                    Status = DeleteCredentials.StatusEnumValue.DELETED
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.As<DeleteCredentials>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.DeleteCredentials("environmentId", "credentialId");

            Assert.IsNotNull(result);
            client.Received().DeleteAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.CredentialId == "credentialId");
            Assert.IsTrue(result.Result.Status == DeleteCredentials.StatusEnumValue.DELETED);
        }
        #endregion

        #region Update Credentials
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCredentials_No_EnvironmentId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateCredentials(null, "credentialId");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCredentials_No_CredentialId()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.UpdateCredentials("environmentId", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCredentials_No_VersionDate()
        {
            BasicAuthenticator authenticator = new BasicAuthenticator("username", "password");
            DiscoveryService service = new DiscoveryService("versionDate", authenticator);
            service.Version = null;
            service.UpdateCredentials("environmentId", "credentialId");
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void UpdateCredentials_Catch_Exception()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                 .Returns(x =>
                 {
                     throw new AggregateException(new ServiceResponseException(Substitute.For<IResponse>(),
                                                                               Substitute.For<HttpResponseMessage>(HttpStatusCode.BadRequest),
                                                                               string.Empty));
                 });

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "2017-11-07";

            service.UpdateCredentials("environmentId", "credentialId");
        }

        [TestMethod]
        public void UpdateCredentials_Success()
        {
            IClient client = CreateClient();

            IRequest request = Substitute.For<IRequest>();
            client.PutAsync(Arg.Any<string>())
                .Returns(request);

            #region Response
            var response = new DetailedResponse<Credentials>()
            {
                Result = new Credentials()
                {
                    SourceType = Credentials.SourceTypeEnumValue.BOX,
                    CredentialDetails = new CredentialDetails()
                    {
                        ClientId = "clientId",
                        ClientSecret = "clientSecret",
                        CredentialType = CredentialDetails.CredentialTypeEnumValue.OAUTH2,
                        EnterpriseId = "enterpriseId",
                        OrganizationUrl = "organizationUrl",
                        Passphrase = "passphrase",
                        Password = "password",
                        PrivateKey = "privateKey",
                        PublicKeyId = "publicKeyId",
                        SiteCollectionPath = "siteCollectionPath",
                        Url = "url",
                        Username = "username"
                    }
                }
            };
            #endregion

            request.WithArgument(Arg.Any<string>(), Arg.Any<string>())
                .Returns(request);
            request.WithBody<Credentials>(new Credentials())
                .Returns(request);
            request.As<Credentials>()
                .Returns(Task.FromResult(response));

            DiscoveryService service = new DiscoveryService(client);
            service.Version = "versionDate";

            var result = service.UpdateCredentials("environmentId", "credentialId");

            Assert.IsNotNull(result);
            client.Received().PutAsync(Arg.Any<string>());
            Assert.IsTrue(result.Result.SourceType == Credentials.SourceTypeEnumValue.BOX);
            Assert.IsNotNull(result.Result.CredentialDetails);
            Assert.IsTrue(result.Result.CredentialDetails.ClientId == "clientId");
            Assert.IsTrue(result.Result.CredentialDetails.ClientSecret == "clientSecret");
            Assert.IsTrue(result.Result.CredentialDetails.CredentialType == CredentialDetails.CredentialTypeEnumValue.OAUTH2);
            Assert.IsTrue(result.Result.CredentialDetails.EnterpriseId == "enterpriseId");
            Assert.IsTrue(result.Result.CredentialDetails.OrganizationUrl == "organizationUrl");
            Assert.IsTrue(result.Result.CredentialDetails.Passphrase == "passphrase");
            Assert.IsTrue(result.Result.CredentialDetails.Password == "password");
            Assert.IsTrue(result.Result.CredentialDetails.PrivateKey == "privateKey");
            Assert.IsTrue(result.Result.CredentialDetails.PublicKeyId == "publicKeyId");
            Assert.IsTrue(result.Result.CredentialDetails.SiteCollectionPath == "siteCollectionPath");
            Assert.IsTrue(result.Result.CredentialDetails.Url == "url");
            Assert.IsTrue(result.Result.CredentialDetails.Username == "username");
        }
        #endregion
        #endregion
    }
}
