#!/bin/bash
COVERAGE_REPORT="$(dotnet test --collect:"XPlat Code Coverage" | tail -n 1 | tr -d '[:space:]')"
reportgenerator -reports:"${COVERAGE_REPORT}" -targetdir:"tests/SharpSearch.Tests/CoverageReport" -reporttypes:Html
