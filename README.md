<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->



<!-- PROJECT LOGO -->
<br />
<div align="center">

[//]: # (  <a href="https://github.com/othneildrew/Best-README-Template">)

[//]: # (    <img src="images/logo.png" alt="Logo" width="80" height="80">)

[//]: # (  </a>)

<h3 align="center">Caddy Manager</h3>

  <p align="center">
    A UI for managing Caddy configuration files
    <br />
    <a href="https://github.com/daothanhduy305/CaddyManager/blob/main/README.md"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/daothanhduy305/CaddyManager/issues/new">Report Bug (label bug)</a>
    &middot;
    <a href="https://github.com/daothanhduy305/CaddyManager/issues/new">Request Feature (label enhancement)</a>

[//]: # (    <a href="https://github.com/othneildrew/Best-README-Template">View Demo</a>)

[//]: # (    &middot;)

  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[//]: # ([![Product Name Screen Shot][product-screenshot]]&#40;https://example.com&#41;)

Caddy Management is an opinionated UI for managing Caddy configuration files. It is designed to be simple and easy to use. The architecture is based on the following principles:
- There has been a Caddy container running on the host machine
- The Caddy container is having its configuration files organized as:
  - A Caddyfile contains the global configuration, and ending with the line `import *.caddy` 
  - Other proxy configurations are saved in individual `*.caddy` files

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

[![DotNet][DotNet]][DotNet-url]
[![Gitea][Gitea]][Gitea-url]
[![Docker][Docker]][Docker-url]

* DotNet 9 with Blazor and [MudBlazor](https://mudblazor.com/)
* Source code and container registry are stored with Gitea
* Docker is used for containerization with DotNet container publishing

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

Given that there has already been a Caddy container running on the host machine, the following steps are required to set up the Caddy Manager using Docker compose.

### Prerequisites

These software are required to be installed on the host machine:
* Docker. Follow the instructions at [https://docs.docker.com/get-docker/](https://docs.docker.com/get-docker/)
* Caddy container. Follow the instructions at [https://caddyserver.com/docs/install](https://caddyserver.com/docs/install#docker)
* The Caddy container is having its configuration files organized at:
  ```yaml
  volumes:
    - /path/to/configs:/etc/caddy
  ```

### Global Caddy configuration

Add this directive at the end of your caddy configuration (global):

```
import *.caddy
```

This is to have the caddy files managed by this application be imported and work as expected.

### Installation with Docker compose

```yaml
services:
  caddy:
    image: caddy:latest
    container_name: caddy
    restart: always
    network_mode: "host"
    security_opt:
      - label:disable
    volumes:
      - /root/compose/caddy/config:/etc/caddy
      - /etc/localtime:/etc/localtime:ro

  caddy-manager:
    image: ghcr.io/daothanhduy305/caddymanager
    container_name: caddy-manager
    restart: always
    environment:
      ASPNETCORE_ENVIRONMENT: "Production"
      CaddyService__ConfigDir: "/config"
      DockerService__CaddyContainerName: "caddy"
    # To have the access to the caddy config file
    user: "1000:1000"
    ports:
      - "8080:8080"
    volumes:
      - /root/compose/caddy/config:/config
      - /var/run/docker.sock:/var/run/docker.sock
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

Currently, the Caddy Manager is able to:
- List all the Caddy configuration files
- Edit the content of the Caddy configuration files by clicking on the file name
- Create and manage the caddy files
- Edit the global Caddy configuration file by using the tab "Global Cadddyfile"
- Restart caddy container on demand
- Parse simple information from the caddy configurations

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [x] Parse the caddy files to get more information, i.e. the domain names, the proxy addresses, etc.

See the [open issues](https://github.com/daothanhduy305/CaddyManager/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


<!-- LICENSE -->
## License

Distributed under the GNU GPLv3 License. See `COPYING` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Ebolo - [@duydao](https://duydao.org) - daothanhduy305@gmail.com

Project Link: [CaddyManager](https://github.com/daothanhduy305/CaddyManager)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

Use this space to list resources you find helpful and would like to give credit to. I've included a few of my favorites to kick things off!

* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Malven's Flexbox Cheatsheet](https://flexbox.malven.co/)
* [Malven's Grid Cheatsheet](https://grid.malven.co/)
* [Img Shields](https://shields.io)
* [GitHub Pages](https://pages.github.com)
* [Font Awesome](https://fontawesome.com)
* [React Icons](https://react-icons.github.io/react-icons/search)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/othneildrew/Best-README-Template.svg?style=for-the-badge
[contributors-url]: https://github.com/othneildrew/Best-README-Template/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/othneildrew/Best-README-Template.svg?style=for-the-badge
[forks-url]: https://github.com/othneildrew/Best-README-Template/network/members
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge
[stars-url]: https://github.com/othneildrew/Best-README-Template/stargazers
[issues-shield]: https://img.shields.io/github/issues/othneildrew/Best-README-Template.svg?style=for-the-badge
[issues-url]: https://github.com/othneildrew/Best-README-Template/issues
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/othneildrew/Best-README-Template/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: images/screenshot.png
[DotNet]: https://img.shields.io/badge/dotnet-563D7C?style=for-the-badge&logo=dotnet&logoColor=white
[DotNet-url]: https://dotnet.microsoft.com/en-us/
[Gitea]: https://img.shields.io/badge/Gitea-green?style=for-the-badge&logo=gitea&logoColor=white
[Gitea-url]: https://about.gitea.com/
[Docker]: https://img.shields.io/badge/Docker-blue?style=for-the-badge&logo=docker&logoColor=white
[Docker-url]: https://www.docker.com/