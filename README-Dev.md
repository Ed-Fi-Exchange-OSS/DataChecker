# Developer README

## File Headers

All C#, SQL, PowerShell, JavaScript files that are copyright of the Ed-Fi
Alliance should have the following file header at the top, with appropriate
comment style:

```csharp
// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
```

Visual Studio Code users can use the "Header Insert" command from the
psioniq.psi-header plugin to edit files without the header. New files created in
VS Code will automatically have the header.

Visual Studio users may like to use the [License Header
Manager](https://marketplace.visualstudio.com/items?itemName=StefanWenig.LicenseHeaderManager)
extension for similar functionality, with a configuration like this:

```none
extensions: designer.cs generated.cs
extensions: .cs .cpp .h .js
// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information on the
// license and copyright ownership.
 
extensions: .aspx .ascx
<%--
   SPDX-License-Identifier: Apache-2.0
   Licensed to the Ed-Fi Alliance under one or more agreements.
   The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
   See the LICENSE and NOTICES files in the project root for more information.
--%>
 
extensions:  .xml .config .xsd .cshtml .html
<!--
   SPDX-License-Identifier: Apache-2.0
   Licensed to the Ed-Fi Alliance under one or more agreements.
   The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
   See the LICENSE and NOTICES files in the project root for more information.
-->

extensions:  .sql
-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.
```