import React, { useState, useRef, useEffect } from 'react';
import { ModRegistrar } from "cs2/modding";
import { bindValue, trigger, useValue } from "cs2/api";

interface CompassSVGProps {
    rotation: number;
}

const CompassSVG: React.FC<CompassSVGProps> = ({ rotation }) => (
    <svg viewBox="0 0 100 100" style={{ width: '40rem', height: '40rem' }}>
        <circle cx="50" cy="50" r="45" fill="#87CEEB" stroke="white" strokeWidth="2" />
        <g transform={`rotate(${rotation} 50 50)`}>
            <path d="M50 15 L54 50 L50 85 L46 50 Z" fill="red" stroke="none" />
            <path d="M50 50 L46 50 L50 85 L54 50 Z" fill="white" stroke="none" />
        </g>
        <circle cx="50" cy="50" r="3" fill="black" />
        <text x="50" y="25" textAnchor="middle" fill="white" fontSize="10">N</text>
        <text x="75" y="52" textAnchor="middle" fill="white" fontSize="10">E</text>
        <text x="50" y="80" textAnchor="middle" fill="white" fontSize="10">S</text>
        <text x="25" y="52" textAnchor="middle" fill="white" fontSize="10">W</text>
    </svg>
);

const CustomMenuButton: React.FC = () => {
    const [showSlider, setShowSlider] = useState<boolean>(false);
    const [rotation, setRotation] = useState<number>(0);
    const sliderRef = useRef<HTMLDivElement>(null);

    const handleClick = (): void => {
        setShowSlider(!showSlider);
        trigger("map_texture", "MainWindowCreate");
    };

    const handleSliderChange = (event: React.MouseEvent<HTMLDivElement>): void => {
        if (sliderRef.current) {
            const rect = sliderRef.current.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const percentage = Math.min(Math.max(x / rect.width, 0), 1);
            const newRotation = Math.round(percentage * 359);
            setRotation(newRotation);
        }
    };

    useEffect(() => {
        const handleMouseMove = (event: MouseEvent): void => {
            if (event.buttons === 1) {  // Left mouse button is pressed
                handleSliderChange(event as unknown as React.MouseEvent<HTMLDivElement>);
            }
        };

        document.addEventListener('mousemove', handleMouseMove);
        return () => {
            document.removeEventListener('mousemove', handleMouseMove);
        };
    }, []);

    return (
        <div className="relative">
            <button
                className="button_ke4 button_ke4 button_h9N w-10 h-10 overflow-hidden"
                onClick={handleClick}
            >
                <div style={{ width: '40rem', height: '40rem' }}  className="w-10 h-10">
                    <CompassSVG rotation={rotation} />
                </div>
            </button>
            {showSlider && (
                <div className="control_Hds" style={{ width: '67.5%', position: 'relative', left: '20rem' }}>
                    <div className="slider-container_Q_K">
                        <div className="slider_KXG slider_pUS horizontal slider_ROT" ref={sliderRef} onClick={handleSliderChange}>
                            <div className="track-bounds_H8_">
                                <div className="range-bounds_lNt" style={{ width: '64.5517rem' }}>
                                    <div className="range_nHO range_iUN" style={{ width: `${(rotation / 359) * 100}%` }}></div>
                                    <div className="thumb-container_aso" style={{ left: `${(rotation / 359) * 100}%` }}>
                                        <div className="thumb_kkL"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <input
                        className="slider-input_DXM input_Wfi"
                        type="text"
                        value={`${rotation}°`}
                        readOnly
                    />
                </div>
            )}
        </div>
    );
};

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append('GameTopLeft', CustomMenuButton);
};

export default register;
