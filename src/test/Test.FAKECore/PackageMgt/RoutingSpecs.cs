﻿using System.Collections.Generic;
using Fake;
using Machine.Specifications;
using Microsoft.FSharp.Core;

namespace Test.FAKECore.PackageMgt
{
    public class when_matching_empty_route
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", "", null),
            new HttpListenerHelper.Route("GET", "test", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "GET", "/");

        It should_find_the_path = () => _result.Value.Route.Path.ShouldEqual("");
        It should_find_the_verb = () => _result.Value.Route.Verb.ShouldEqual("GET");
    }

    public class when_matching_nonexisting_route
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", "test/tester", null),
            new HttpListenerHelper.Route("POST", "test", null),
            new HttpListenerHelper.Route("GET", "test", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "GET", "/test/blub");

        It should_not_find_a_route = () => _result.ShouldBeNone();
    }

    public class when_matching_existing_route
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", "test/tester", null),
            new HttpListenerHelper.Route("POST", "test", null),
            new HttpListenerHelper.Route("GET", "test", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "GET", "/test");

        It should_find_the_path = () => _result.Value.Route.Path.ShouldEqual("test");
        It should_find_the_verb = () => _result.Value.Route.Verb.ShouldEqual("GET");
    }

    public class when_matching_route_with_placeholder
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", "test/tester", null),
            new HttpListenerHelper.Route("POST", "test/([^/]+)/", null),
            new HttpListenerHelper.Route("GET", "test", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "POST", "/test/blub");

        It should_find_the_parameters = () => _result.Value.Parameters[0].ShouldEqual("blub");
        It should_find_the_path = () => _result.Value.Route.Path.ShouldEqual("test/([^/]+)/");
        It should_find_the_verb = () => _result.Value.Route.Verb.ShouldEqual("POST");
    }

    public class when_matching_route_with_deployment_package_placeholder
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", @"deployments/([^/]+)/\?status=active", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "GET", "deployments/fake_website/?status=active");

        It should_find_the_parameters = () => _result.Value.Parameters[0].ShouldEqual("fake_website");
    }

    public class when_matching_route_with_two_placeholders
    {
        static FSharpOption<HttpListenerHelper.RouteResult> _result;
        static List<HttpListenerHelper.Route> _routes;

        Establish context = () => _routes = new List<HttpListenerHelper.Route>
        {
            new HttpListenerHelper.Route("GET", "test/tester", null),
            new HttpListenerHelper.Route("POST", "test/([^/]+)/", null),
            new HttpListenerHelper.Route("GET", @"rollback/(.+)\?version=([^/]+)", null),
        };

        Because of = () => _result = HttpListenerHelper.matchRoute(_routes, "GET", "/rollback/FAKE?version=1.0.0");

        It should_find_the_first_parameters = () => _result.Value.Parameters[0].ShouldEqual("fake");
        It should_find_the_second_parameters = () => _result.Value.Parameters[1].ShouldEqual("1.0.0");
        It should_find_the_path = () => _result.Value.Route.Path.ShouldEqual(@"rollback/(.+)\?version=([^/]+)");
        It should_find_the_verb = () => _result.Value.Route.Verb.ShouldEqual("GET");
    }
}