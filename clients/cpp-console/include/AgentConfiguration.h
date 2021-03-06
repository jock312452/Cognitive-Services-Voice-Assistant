// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma once

#include <string>
#include <memory>
#include <speechapi_cxx.h>

//the pragma here suppresses warnings from the 3rd party header
#pragma warning(push, 0)
#pragma warning (disable : 26451)
#pragma warning (disable : 26444)
#pragma warning (disable : 28020)
#pragma warning (disable : 26495)
#include "json.hpp"
#pragma warning(pop)

enum class AgentDialogType
{
    Undefined,
    DirectLineSpeech,
    CustomCommands
};

enum class AgentConfigurationLoadResult
{
    Undefined,
    Success,
    FileNotFound,
    FileNotParsed,
    BadDialogType,
    BadSpeechKey,
    MissingRegion,
    RegionWithCustom,
    UnknownFailure
};

class AgentConfiguration
{
public:
    std::string _commandsAppId;
    std::string _speechKey;
    std::string _speechRegion;
    std::string _customVoiceIds;
    std::string _customSpeechId;
    std::string _customEndpoint;
    std::string _keywordModelPath;
    std::string _keywordDisplayName;
    unsigned int _volume = 0;

    AgentConfiguration();
    static std::shared_ptr<AgentConfiguration> LoadFromFile(const std::string& path);


public:
    const AgentConfigurationLoadResult LoadResult() { return _loadResult; }
    const std::string KeywordModel() { return _keywordModelPath; }
    const std::string KeywordDisplayName() { return _keywordDisplayName; }
    std::shared_ptr<Microsoft::CognitiveServices::Speech::Dialog::DialogServiceConfig> AsDialogServiceConfig();
    std::shared_ptr<Microsoft::CognitiveServices::Speech::Dialog::DialogServiceConfig> CreateDialogServiceConfig();

private:
    AgentConfigurationLoadResult _loadResult;
    nlohmann::json _configJson;
    std::shared_ptr<Microsoft::CognitiveServices::Speech::Dialog::DialogServiceConfig> _dialogServiceConfig;

};