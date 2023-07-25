# SharpSearch

[![Test Status](https://github.com/tg2648/SharpSearch/actions/workflows/test.yaml/badge.svg?branch=main)](https://github.com/tg2648/SharpSearch/actions/workflows/test.yaml)
[![codecov](https://codecov.io/gh/tg2648/SharpSearch/branch/main/graph/badge.svg?token=6A4VDT44V5)](https://codecov.io/gh/tg2648/SharpSearch)

SharpSearch is a local search engine.

## Sample usage

Add file(s) or folder(s) to the index. If an existing index doesn't exist in `~/.config/SharpSearch` a new index will be created.

```
./SharpSearch add file_path folder_path
```

Find `n` documents that are the most relevant to the search query. The default is 10 documents.
The basic principles of [information retrieval](https://en.wikipedia.org/wiki/Tf%E2%80%93idf) are used to rank the documents.

```
./SharpSearch query "search term" --n 5
```

## Installation

Currently only installation from source is supported, which requires .NET 6.0.x or 7.0.x:

```
cd src/SharpSearch
dotnet publish -c Release -r osx-arm64 --self-contained false
```

The executable will be in `src/SharpSearch/bin/Release/net7.0/<OS>/publish/`.
