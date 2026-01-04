import React, { useEffect, useState } from "react";
import axios from "axios";

interface City {
    id: number;
    name: string;
    countryName: string;
}

const CitiesList: React.FC = () => {
    const [cities, setCities] = useState<City[]>([]);

    useEffect(() => {
        axios.get<City[]>("http://localhost:5263/api/cities")
            .then(res => setCities(res.data))
            .catch(err => console.error(err));
    }, []);

    return (
        <div>
            <h2>Cities</h2>
            <ul>
                {cities.map(c => (
                    <li key={c.id}>{c.name} ({c.countryName})</li>
                ))}
            </ul>
        </div>
    );
};

export default CitiesList;