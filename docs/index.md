# PSRule

PSRule is an open-source, general-purpose rules engine built on top of PowerShell and maintained on GitHub.

PSRule provides an easy way to:

- Define reusable business rules like scripts.
- Validate PowerShell objects and infrastructure code.

Features of PSRule include:

- [Extensible](features.md#extensible) - Use PowerShell, a flexible scripting language.
- [Cross-platform](features.md#cross-platform) - Run on MacOS, Linux, and Windows.
- [Reusable](features.md#reusable) - Share rules across teams or organizations.
- [Recommendations](features.md#recommendations) - Include detailed instructions to remediate issues.

## Support

This project uses GitHub Issues to track bugs and feature requests.
Please search the existing issues before filing new issues to avoid duplicates.

- For new issues, file your bug or feature request as a new [issue].
- For help, discussion, and support questions about using this project, join or start a [discussion].

Support for this project/ product is limited to the resources listed above.

## Using locally

You can download and install the PSRule module from the PowerShell Gallery.

Module | Description | Downloads / instructions
------ | ----------- | ------------------------
PSRule | Validate infrastructure as code (IaC) and objects using PowerShell rules. | [latest][module] / [instructions][install]

![module-ci-badge] ![module-version-badge] ![module-downloads-badge]

## Visual Studio Code extension

You can download and install the companion extension for Visual Studio Code from the Visual Studio Marketplace.
The extension is available to download in to release channels.

Channel | Description | Version/ installs
------- | ----------- | ---
[Preview][ext-preview] | More frequent releases but more likely to contain bugs. | [![Preview][ext-preview-version-badge]][ext-preview] ![ext-preview-installs-badge]
[Stable][ext-stable] | Less frequent releases, with more user testing, experimental features are disabled. | [![Stable][ext-stable-version-badge]][ext-stable] ![ext-stable-installs-badge]

## Azure DevOps extension

You can install the extension for Azure Pipelines from the Visual Studio Marketplace.
This extension allows you to run PSRule analysis without having to use PowerShell directly.

Extension | Description | Downloads / instructions
--------- | ----------- | ------------------------
PSRule    | Validate infrastructure as code (IaC) and DevOps repositories using Azure Pipelines. | [latest][extension-pipelines] / [instructions][install]

![extension-pipelines-ci-badge] ![extension-pipelines-version-badge]

## GitHub action

You can use PSRule with in a workflow with GitHub Actions.
This action allows you to run PSRule analysis without having to use PowerShell directly.

Action | Description | Downloads / instructions
------ | ----------- | ------------------------
PSRule | Validate infrastructure as code (IaC) and DevOps repositories using GitHub Actions. | [latest][extension-github] / [instructions][install]

![extension-github-ci-badge] ![extension-github-version-badge]

## Additional modules

You can optionally download and install the following modules from the PowerShell Gallery.

Module                    | Description | Version / downloads
------                    | ----------- | -------------------
[PSRule.Rules.Azure]      | A suite of rules to validate Azure resources and infrastructure as code (IaC) using PSRule. | [![rules-azure-version-badge]][rules-azure-version-module] [![rules-azure-downloads-badge]][rules-azure-version-module]
[PSRule.Rules.Kubernetes] | A suite of rules to validate Kubernetes resources using PSRule. | [![rules-kubernetes-version-badge]][rules-kubernetes-version-module] [![rules-kubernetes-downloads-badge]][rules-kubernetes-version-module]
[PSRule.Rules.CAF]        | A suite of rules to validate Azure resources against the Cloud Adoption Framework (CAF) using PSRule. | [![rules-caf-version-badge]][rules-caf-version-module] [![rules-caf-downloads-badge]][rules-caf-version-module]
[PSRule.Rules.GitHub]     | A suite of rules to validate GitHub repositories using PSRule. | [![rules-github-version-badge]][rules-github-version-module] [![rules-github-downloads-badge]][rules-github-version-module]
[PSRule.Rules.MSFT.OSS]   | A suite of rules to validate repositories against Microsoft Open Source Software (OSS) requirements. | [![rules-msft-oss-version-badge]][rules-msft-oss-version-module] [![rules-msft-oss-downloads-badge]][rules-msft-oss-version-module]

[issue]: https://github.com/Microsoft/PSRule/issues
[discussion]: https://github.com/microsoft/PSRule/discussions
[install]: install-instructions.md
[module]: https://www.powershellgallery.com/packages/PSRule
[module-ci-badge]: https://dev.azure.com/bewhite/PSRule/_apis/build/status/PSRule-CI?branchName=main
[module-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.svg?label=PowerShell%20Gallery&color=brightgreen
[module-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.svg?color=brightgreen
[ext-preview]: https://marketplace.visualstudio.com/items?itemName=bewhite.psrule-vscode-preview
[ext-preview-version-badge]: https://vsmarketplacebadge.apphb.com/version/bewhite.psrule-vscode-preview.svg
[ext-preview-installs-badge]: https://vsmarketplacebadge.apphb.com/installs-short/bewhite.psrule-vscode-preview.svg
[ext-stable]: https://marketplace.visualstudio.com/items?itemName=bewhite.psrule-vscode
[ext-stable-version-badge]: https://vsmarketplacebadge.apphb.com/version/bewhite.psrule-vscode.svg
[ext-stable-installs-badge]: https://vsmarketplacebadge.apphb.com/installs-short/bewhite.psrule-vscode.svg
[extension-pipelines]: https://marketplace.visualstudio.com/items?itemName=bewhite.ps-rule
[extension-pipelines-ci-badge]: https://dev.azure.com/bewhite/PSRule-pipelines/_apis/build/status/PSRule-pipelines-CI?branchName=main
[extension-pipelines-version-badge]: https://vsmarketplacebadge.apphb.com/version/bewhite.ps-rule.svg
[extension-github]: https://github.com/marketplace/actions/psrule
[extension-github-ci-badge]: https://img.shields.io/github/workflow/status/microsoft/ps-rule/Build?label=GitHub%20Actions&color=brightgreen
[extension-github-version-badge]: https://img.shields.io/github/v/release/microsoft/ps-rule?sort=semver&label=release&color=brightgreen
[rules-azure-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.Rules.Azure.svg?label=PowerShell%20Gallery&color=brightgreen
[rules-azure-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.Rules.Azure.svg?color=brightgreen
[rules-azure-version-module]: https://www.powershellgallery.com/packages/PSRule.Rules.Azure
[rules-kubernetes-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.Rules.Kubernetes.svg?label=PowerShell%20Gallery&color=brightgreen
[rules-kubernetes-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.Rules.Kubernetes.svg?color=brightgreen
[rules-kubernetes-version-module]: https://www.powershellgallery.com/packages/PSRule.Rules.Kubernetes
[rules-caf-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.Rules.CAF.svg?label=PowerShell%20Gallery&color=brightgreen
[rules-caf-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.Rules.CAF.svg?color=brightgreen
[rules-caf-version-module]: https://www.powershellgallery.com/packages/PSRule.Rules.CAF
[rules-github-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.Rules.GitHub.svg?label=PowerShell%20Gallery&color=brightgreen
[rules-github-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.Rules.GitHub.svg?color=brightgreen
[rules-github-version-module]: https://www.powershellgallery.com/packages/PSRule.Rules.GitHub
[rules-msft-oss-version-badge]: https://img.shields.io/powershellgallery/v/PSRule.Rules.MSFT.OSS.svg?label=PowerShell%20Gallery&color=brightgreen
[rules-msft-oss-downloads-badge]: https://img.shields.io/powershellgallery/dt/PSRule.Rules.MSFT.OSS.svg?color=brightgreen
[rules-msft-oss-version-module]: https://www.powershellgallery.com/packages/PSRule.Rules.MSFT.OSS
[PSRule.Rules.Azure]: https://github.com/microsoft/PSRule.Rules.Azure
[PSRule.Rules.Kubernetes]: https://github.com/microsoft/PSRule.Rules.Kubernetes
[PSRule.Rules.CAF]: https://github.com/microsoft/PSRule.Rules.CAF
[PSRule.Rules.GitHub]: https://github.com/microsoft/PSRule.Rules.GitHub
[PSRule.Rules.MSFT.OSS]: https://github.com/microsoft/PSRule.Rules.MSFT.OSS
